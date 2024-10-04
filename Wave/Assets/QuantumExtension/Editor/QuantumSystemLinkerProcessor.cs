using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Quantum;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.UnityLinker;
using UnityEngine;

namespace QuantumExtension.Editor
{
    public class QuantumSystemLinkerProcessor : IUnityLinkerProcessor
    {
        public int callbackOrder => 0;

        private const string LinkXmlName = "QuantumSystem";

        public string GenerateAdditionalLinkXmlFile(BuildReport report, UnityLinkerBuildPipelineData data)
        {
            var currentAssemblyName = typeof(Quantum.ZombieSpawnAreaSystem).Assembly.GetName().Name;
            
            var typesByAssemblies = new Dictionary<System.Reflection.Assembly, Type[]>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                try
                {
                    if (assembly.GetName().Name != currentAssemblyName &&
                        assembly.GetReferencedAssemblies().All(x => x.Name != currentAssemblyName))
                        continue;

                    var types = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(SystemBase))).ToArray();
                    if (types.Length > 0)
                        typesByAssemblies.Add(assembly, types);
                }
                catch (ReflectionTypeLoadException)
                {
                    Debug.LogWarning($"アセンブリの型をロードできませんでした: {assembly.FullName}");
                }
            }

            var sb = new StringBuilder();
            sb.AppendLine("<linker>");

            foreach (var assembly in typesByAssemblies.Keys.OrderBy(a => a.GetName().Name))
            {
                sb.AppendLine($"  <assembly fullname=\"{assembly.GetName().Name}\">");

                var types = typesByAssemblies[assembly];
                foreach (var type in types.OrderBy(t => t.FullName))
                {
                    sb.AppendLine(
                        $"    <type fullname=\"{FormatForXml(ToCecilName(type.FullName))}\" preserve=\"all\"/>");
                }

                sb.AppendLine("  </assembly>");
            }

            sb.AppendLine("</linker>");

            var path = Path.Combine(Application.dataPath, "..", "Temp", $"{LinkXmlName}.link.xml");

            File.WriteAllText(path, sb.ToString());
            return path;
        }

        private static string ToCecilName(string fullTypeName)
        {
            return fullTypeName.Replace('+', '/');
        }

        private static string FormatForXml(string value)
        {
            return value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }
    }
}