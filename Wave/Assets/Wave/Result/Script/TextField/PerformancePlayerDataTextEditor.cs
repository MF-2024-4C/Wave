using System;
using System.Reflection;
using UnityEditor;

namespace Wave.Result
{
    [CustomEditor(typeof(PerformancePlayerDataText))]
    public class PerformancePlayerDataTextEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            PerformancePlayerDataText completeBoard = (PerformancePlayerDataText)target;
            DrawDefaultInspector();

            Type dataType = typeof(ResultPlayerData);
            PropertyInfo[] fields = dataType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (fields.Length > 0)
            {
                //フィールド名のリストを作成
                string[] fieldNames = Array.ConvertAll(fields, field=> field.Name);
                int currentIndex = Array.IndexOf(fieldNames, completeBoard.fieldName);
                
                //フィールド名をプルダウンメニューとして表示
                int selectedIndex = EditorGUILayout.Popup("Field Name", currentIndex, fieldNames);
                if(selectedIndex >= 0 && selectedIndex < fieldNames.Length)
                {
                    completeBoard.fieldName = fieldNames[selectedIndex];
                    
                    Type propertyType = fields[selectedIndex].PropertyType;
                    EditorGUILayout.LabelField("Property Type", propertyType.Name);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("ResultPlayerDataにデータがありません。", MessageType.Warning);
            }
        }
    }
}