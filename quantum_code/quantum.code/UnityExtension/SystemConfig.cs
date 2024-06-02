using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;

namespace Quantum
{
    [Serializable]
    public struct SerializableType : IEquatable<SerializableType>
    {
        public string AssemblyQualifiedName;

        private static readonly Regex s_shortNameRegex =
            new Regex(", (Version|Culture|PublicKeyToken)=[^, \\]]+", RegexOptions.Compiled);

        public bool IsValid => !string.IsNullOrEmpty(this.AssemblyQualifiedName);

        public SerializableType(Type type) => this.AssemblyQualifiedName = type.AssemblyQualifiedName;

        public Type Value => new SerializableType<object>()
        {
            AssemblyQualifiedName = this.AssemblyQualifiedName
        }.Value;

        public SerializableType AsShort() => new SerializableType()
        {
            AssemblyQualifiedName = SerializableType.GetShortAssemblyQualifiedName(this.AssemblyQualifiedName)
        };

        public static implicit operator SerializableType(Type type) => new SerializableType(type);

        public static implicit operator Type(SerializableType serializableType) => serializableType.Value;

        public bool Equals(SerializableType other) => this.AssemblyQualifiedName == other.AssemblyQualifiedName;

        public override bool Equals(object obj) => obj is SerializableType other && this.Equals(other);

        public override int GetHashCode() =>
            this.AssemblyQualifiedName == null ? 0 : this.AssemblyQualifiedName.GetHashCode();

        public static string GetShortAssemblyQualifiedName(Type type) =>
            SerializableType.GetShortAssemblyQualifiedName(type.AssemblyQualifiedName ??
                                                           throw new InvalidOperationException());

        internal static string GetShortAssemblyQualifiedName(string assemblyQualifiedName) =>
            SerializableType.s_shortNameRegex.Replace(assemblyQualifiedName, string.Empty);
    }

    [Serializable]
    public struct SerializableType<BaseType> : IEquatable<SerializableType<BaseType>>
    {
        private static Dictionary<string, object> _cache = new Dictionary<string, object>();
        public string AssemblyQualifiedName;

        public override string ToString()
        {
            return AssemblyQualifiedName;
        }
        public bool IsValid => !string.IsNullOrEmpty(this.AssemblyQualifiedName);

        public SerializableType(Type type) => this.AssemblyQualifiedName = type.AssemblyQualifiedName;

        public SerializableType<BaseType> AsShort() => new SerializableType<BaseType>()
        {
            AssemblyQualifiedName = SerializableType.GetShortAssemblyQualifiedName(this.AssemblyQualifiedName)
        };

        public Type Value
        {
            get
            {
                if (string.IsNullOrEmpty(this.AssemblyQualifiedName))
                    return (Type)null;
                object obj;
                if (!SerializableType<BaseType>._cache.TryGetValue(this.AssemblyQualifiedName, out obj))
                {
                    try
                    {
                        obj = (object)Type.GetType(this.AssemblyQualifiedName, true);
                    }
                    catch (Exception ex)
                    {
                        obj = (object)ExceptionDispatchInfo.Capture(ex);
                    }

                    SerializableType<BaseType>._cache.Add(this.AssemblyQualifiedName, obj);
                }

                if (obj is ExceptionDispatchInfo exceptionDispatchInfo)
                    exceptionDispatchInfo.Throw();
                Type type = (Type)obj;
                if (type == (Type)null)
                    throw new Exception("Type " + this.AssemblyQualifiedName + " not found");
                return type.IsSubclassOf(typeof(BaseType))
                    ? type
                    : throw new Exception(string.Format("Type mismatch: {0} must inherit from {1}", (object)type,
                        (object)typeof(BaseType)));
            }
        }

        public static implicit operator SerializableType<BaseType>(Type type) => new SerializableType<BaseType>(type);

        public static implicit operator Type(SerializableType<BaseType> serializableType) => serializableType.Value;

        public bool Equals(SerializableType<BaseType> other) =>
            this.AssemblyQualifiedName == other.AssemblyQualifiedName;

        public override bool Equals(object obj) => obj is SerializableType<BaseType> other && this.Equals(other);

        public override int GetHashCode() =>
            this.AssemblyQualifiedName == null ? 0 : this.AssemblyQualifiedName.GetHashCode();
    }

    [AssetObjectConfig(GenerateAssetResetMethod = false, GenerateLinkingScripts = false)]
    public partial class SystemConfig : AssetObject
    {
        [Serializable]
        public abstract class SystemEntryBase
        {
            /// <summary>
            /// System type name. Use typeof(SystemBase).FullName to get a valid name progamatically. E.g. Quantum.Core.SystemSignalsOnly.
            /// </summary>
            public SerializableType<SystemBase> SystemType;

            /// <summary>
            /// Optional System name. If set, then the SystemType class needs to have a matching contructor.
            /// </summary>
            public string SystemName;

            /// <summary>
            /// Start system disabled.
            /// Set <see cref="SystemBase.StartEnabled"/> accordingly. The value is inversed to have a better default value in Unity inspectors.
            /// </summary>
            public bool StartDisabled;

            public abstract IReadOnlyList<SystemEntryBase> GetChildren();
        }

        public abstract class SystemEntryBase<T> : SystemEntryBase where T : SystemEntryBase, new()
        {
            public List<T> Children = new List<T>();
            public override IReadOnlyList<SystemEntryBase> GetChildren() => Children;

            public T AddSystem<TSystem>(string name = null, bool enabled = true) where TSystem : SystemBase
            {
                var entry = new T()
                {
                    SystemType = typeof(TSystem),
                    StartDisabled = !enabled,
                    SystemName = name
                };
                Children.Add(entry);
                return entry;
            }
        }

        [Serializable]
        public class SystemEntry : SystemEntryBase<SubSystemEntry>
        {
        }

        [Serializable]
        public class SubSystemEntry : SystemEntryBase<SubSubSystemEntry>
        {
        }

        [Serializable]
        public class SubSubSystemEntry : SystemEntryBase
        {
            public override IReadOnlyList<SystemEntryBase> GetChildren()
            {
                return Array.Empty<SystemEntryBase>();
            }
        }
        
        [NamedElement("SystemType")]
        public List<SystemEntry> Entries = new();

        public static List<SystemBase> CreateSystems(SystemConfig config)
        {
            Assert.Always(config != null, "SystemsConfig is invalid.");

            var result = new List<SystemBase>();

            for (int i = 0; i < config.Entries.Count; i++)
            {
                try
                {
                    result.Add(CreateSystems<SystemBase>(config.Entries[i]));
                }
                catch (Exception e)
                {
                    Log.Error(
                        $"Creating system failed from asset '{config.Path}' at index {i} with error: {e.Message}");
                }
            }

            return result;
        }

        private static SystemBase CreateSystems<RequiredBaseType>(SystemEntryBase entry)
        {
            if (entry.SystemType.AssemblyQualifiedName.Contains(", Quantum.Game, Version"))
            {
                throw new Exception(
                    "The assembly 'Quantum.Game' is not supported anymore, edit the SystemsConfig file and replace 'Quantum.Game' with 'Quantum.Simulation'");
            }

            var type = entry.SystemType.Value;

            Assert.Always(type != null, "SystemType not set");
            Assert.Always(type.IsAbstract == false, "Cannot create abstract SystemType {0}", type);
            Assert.Always(typeof(RequiredBaseType).IsAssignableFrom(type), "System type {0} must be derived from {1}",
                type, typeof(RequiredBaseType).Name);

            var result = default(SystemBase);
            var childrenEntries = entry.GetChildren();

            if (typeof(SystemGroup).IsAssignableFrom(type))
            {
                Assert.Always(childrenEntries != null,
                    "SystemType {0} is derived from SystemGroup and requires the Children parameter to be not null.",
                    type);
                ;
                var children = new List<SystemBase>(childrenEntries.Count);
                for (int i = 0; i < childrenEntries.Count; i++)
                {
                    children.Add(CreateSystems<SystemBase>(childrenEntries[i]) as SystemBase);
                }

                result = Create(type, entry.SystemName, children.ToArray());
            }
            else if (typeof(SystemMainThreadGroup).IsAssignableFrom(type))
            {
                Assert.Always(childrenEntries != null,
                    "SystemType {0} is derived from SystemMainThreadGroup and requires the Children parameter to be not null.",
                    type);
                ;
                var children = new List<SystemMainThread>(childrenEntries.Count);
                for (int i = 0; i < childrenEntries.Count; i++)
                {
                    children.Add(CreateSystems<SystemMainThread>(childrenEntries[i]) as SystemMainThread);
                }

                result = Create(type, entry.SystemName, children.ToArray());
            }
            else
            {
                result = Create<SystemBase>(type, entry.SystemName, null);
            }

            result.StartEnabled = !entry.StartDisabled;
            return result;
        }

        private static SystemBase Create<ChildrenType>(Type type, string name, ChildrenType[] children)
            where ChildrenType : SystemBase
        {
            // Conventions are: (), (name), (name, children)
            if (string.IsNullOrEmpty(name) == false && children != null)
            {
                Assert.Always(
                    type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null,
                        new Type[] { typeof(string), typeof(ChildrenType[]) }, null) != null,
                    "SystemType {0} does not have the contructor for (string, {1}).", type, typeof(ChildrenType));
                return Activator.CreateInstance(type, name, children) as SystemBase;
            }
            else if (string.IsNullOrEmpty(name) == false)
            {
                Assert.Always(
                    type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null,
                        new Type[] { typeof(string) }, null) != null,
                    "SystemType {0} does not have the contructor for (string).", type);
                return Activator.CreateInstance(type, name) as SystemBase;
            }
            else
            {
                Assert.Always(type.GetConstructor(Type.EmptyTypes) != null,
                    "SystemType {0} does not have a default contructor", type);
                return Activator.CreateInstance(type) as SystemBase;
            }
        }

        public SystemEntry AddSystem<T>(string name = null, bool enabled = true) where T : SystemBase
        {
            return AddSystem(typeof(T), name, enabled);
        }

        public SystemEntry AddSystem(Type systemType, string name = null, bool enabled = true)
        {
            if (systemType == null) throw new ArgumentNullException(nameof(systemType));

            var entry = new SystemEntry()
            {
                SystemType = systemType,
                StartDisabled = !enabled,
                SystemName = name
            };
            Entries.Add(entry);
            return entry;
        }
    }
}