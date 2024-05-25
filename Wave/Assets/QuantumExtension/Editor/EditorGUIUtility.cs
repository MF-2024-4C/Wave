using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Quantum.Editor
{
    [UnityEditor.InitializeOnLoad]
    public static class EditorGUIUtility
    {
        private static readonly ReflectionUtils.StaticAccessor<int> s_LastControlID =
            typeof(UnityEditor.EditorGUIUtility).CreateStaticFieldAccessor<int>(nameof(s_LastControlID));

        private static readonly ReflectionUtils.StaticAccessor<float> _contentWidth =
            typeof(UnityEditor.EditorGUIUtility).CreateStaticPropertyAccessor<float>(nameof(contextWidth));

        public static int LastControlID => s_LastControlID.GetValue();
        public static float contextWidth => _contentWidth.GetValue();

        public delegate UnityEngine.Object GetScriptDelegate(string scriptClass);

        public delegate Texture2D GetIconForObjectDelegate(UnityEngine.Object obj);

        public delegate GUIContent TempContentDelegate(string text);

        public delegate Texture2D GetHelpIconDelegate(MessageType type);

        public static readonly GetScriptDelegate GetScript =
            typeof(UnityEditor.EditorGUIUtility).CreateMethodDelegate<GetScriptDelegate>(nameof(GetScript));

        public static readonly GetIconForObjectDelegate GetIconForObject =
            typeof(UnityEditor.EditorGUIUtility).CreateMethodDelegate<GetIconForObjectDelegate>(
                nameof(GetIconForObject));

        public static readonly TempContentDelegate TempContent =
            typeof(UnityEditor.EditorGUIUtility).CreateMethodDelegate<TempContentDelegate>(nameof(TempContent));

        public static readonly GetHelpIconDelegate GetHelpIcon =
            typeof(UnityEditor.EditorGUIUtility).CreateMethodDelegate<GetHelpIconDelegate>(nameof(GetHelpIcon));
    }
    public static partial class QuantumEditorGUI
    {
        public const string ScriptPropertyName = "m_Script";

        private const int IconHeight = 14;

        public static readonly GUIContent WhitespaceContent = new(" ");

        public static Color PrefebOverridenColor => new(1f / 255f, 153f / 255f, 235f / 255f, 0.75f);

        public static float FoldoutWidth => 16.0f;

        public static Rect Decorate(Rect rect, string tooltip, MessageType messageType, bool hasLabel = false,
            bool drawBorder = true, bool drawButton = true)
        {
            if (hasLabel)
            {
                rect.xMin += UnityEditor.EditorGUIUtility.labelWidth;
            }

            var content = UnityEditor.EditorGUIUtility.TrTextContentWithIcon(string.Empty, tooltip, messageType);
            var iconRect = rect;
            iconRect.width = Mathf.Min(16, rect.width);
            iconRect.xMin -= iconRect.width;

            iconRect.y += (iconRect.height - IconHeight) / 2;
            iconRect.height = IconHeight;

            if (drawButton)
            {
                using (new QuantumEditorGUI.EnabledScope(true))
                {
                    GUI.Label(iconRect, content, GUIStyle.none);
                }
            }

            //GUI.Label(iconRect, content, new GUIStyle());

            if (drawBorder)
            {
                Color borderColor;
                switch (messageType)
                {
                    case MessageType.Warning:
                        borderColor = new Color(1.0f, 0.5f, 0.0f);
                        break;
                    case MessageType.Error:
                        borderColor = new Color(1.0f, 0.0f, 0.0f);
                        break;
                    default:
                        borderColor = new Color(1f, 1f, 1f, .0f);
                        break;
                }

                GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill, false, 0, borderColor, 1.0f,
                    1.0f);
            }

            return iconRect;
        }

        public static void AppendTooltip(string tooltip, ref GUIContent label)
        {
            if (!string.IsNullOrEmpty(tooltip))
            {
                label = new GUIContent(label);
                if (string.IsNullOrEmpty(label.tooltip))
                {
                    label.tooltip = tooltip;
                }
                else
                {
                    label.tooltip += "\n\n" + tooltip;
                }
            }
        }

        public static void ScriptPropertyField(UnityEditor.Editor editor)
        {
            ScriptPropertyField(editor.serializedObject);
        }

        public static void ScriptPropertyField(SerializedObject obj)
        {
            var scriptProperty = obj.FindProperty(ScriptPropertyName);
            if (scriptProperty == null) return;
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(scriptProperty);
            }
        }

        public static float GetLinesHeight(int count)
        {
            return count * (UnityEditor.EditorGUIUtility.singleLineHeight) + (count - 1) * UnityEditor.EditorGUIUtility.standardVerticalSpacing;
        }

        public static float GetLinesHeightWithNarrowModeSupport(int count)
        {
            if (!UnityEditor.EditorGUIUtility.wideMode)
            {
                count++;
            }

            return count * (UnityEditor.EditorGUIUtility.singleLineHeight) + (count - 1) * UnityEditor.EditorGUIUtility.standardVerticalSpacing;
        }

        public static System.Type GetDrawerTypeIncludingWorkarounds(System.Attribute attribute)
        {
            var drawerType = UnityInternal.ScriptAttributeUtility.GetDrawerTypeForType(attribute.GetType(), false);
            if (drawerType == null)
            {
                return null;
            }

            return drawerType;
        }

        public static void DisplayTypePickerMenu(Rect position, Type[] baseTypes, Action<Type> callback,
            Func<Type, bool> filter, string noneOptionLabel = "[None]", bool groupByNamespace = true,
            Type selectedType = null)
        {
            var types = new List<Type>();

            foreach (var baseType in baseTypes)
            {
                types.AddRange(TypeCache.GetTypesDerivedFrom(baseType).Where(filter));
                if (filter(baseType))
                {
                    types.Add(baseType);
                }
            }

            if (baseTypes.Length > 1)
            {
                types = types.Distinct().ToList();
            }

            types.Sort((a, b) => string.CompareOrdinal(a.FullName, b.FullName));


            List<GUIContent> menuOptions = new List<GUIContent>();
            var actualTypes = new Dictionary<string, System.Type>();

            menuOptions.Add(new GUIContent(noneOptionLabel));
            actualTypes.Add(noneOptionLabel, null);

            int selectedIndex = -1;

            foreach (var ns in types.GroupBy(
                         x => string.IsNullOrEmpty(x.Namespace) ? "[Global Namespace]" : x.Namespace))
            {
                foreach (var t in ns)
                {
                    var nameWithoutNamespace = t.FullName;
                    if (string.IsNullOrEmpty(nameWithoutNamespace))
                    {
                        continue;
                    }

                    if (t.Namespace != null && t.Namespace.Length > 0)
                    {
                        nameWithoutNamespace = nameWithoutNamespace.Substring(t.Namespace.Length + 1);
                    }

                    string path;
                    if (groupByNamespace)
                    {
                        path = ns.Key + "/" + nameWithoutNamespace;
                    }
                    else
                    {
                        path = t.FullName;
                    }

                    if (actualTypes.ContainsKey(path))
                    {
                        continue;
                    }

                    menuOptions.Add(new GUIContent(path));
                    actualTypes.Add(path, t);

                    if (selectedType == t)
                    {
                        selectedIndex = menuOptions.Count - 1;
                    }
                }
            }

            EditorUtility.DisplayCustomMenu(position, menuOptions.ToArray(), selectedIndex,
                (userData, options, selected) =>
                {
                    var path = options[selected];
                    var newType = ((Dictionary<string, System.Type>)userData)[path];
                    callback(newType);
                }, actualTypes);
        }


        public static void DisplayTypePickerMenu(Rect position, Type[] baseTypes, Action<Type> callback,
            string noneOptionLabel = "[None]", bool groupByNamespace = true, Type selectedType = null,
            bool enableAbstract = false, bool enableGenericTypeDefinitions = false)
        {
            DisplayTypePickerMenu(position, baseTypes, callback, x =>
                (enableAbstract || !x.IsAbstract) &&
                (enableGenericTypeDefinitions || !x.IsGenericTypeDefinition));
        }

        public static void DisplayTypePickerMenu(Rect position, Type baseType, Action<Type> callback,
            string noneOptionLabel = "[None]", bool groupByNamespace = true, Type selectedType = null,
            bool enableAbstract = false, bool enableGenericTypeDefinitions = false)
        {
            DisplayTypePickerMenu(position, new[] { baseType }, callback, x =>
                (enableAbstract || !x.IsAbstract) &&
                (enableGenericTypeDefinitions || !x.IsGenericTypeDefinition));
        }
        
        
        public struct EnabledScope: IDisposable {
            private readonly bool value;

            public EnabledScope(bool enabled) {
                value       = GUI.enabled;
                GUI.enabled = enabled;
            }

            public void Dispose() {
                GUI.enabled = value;
            }
        }
    }
}