using System;
using System.Reflection;
using UnityEditor;

namespace Wave.Result
{
    [CustomEditor(typeof(ResultGameDataText))]
    public class ResultGameDataTextEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ResultGameDataText completeBoard = (ResultGameDataText)target;
            DrawDefaultInspector();

            Type dataType = typeof(ResultGameData);
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
                EditorGUILayout.HelpBox("ResultGameDataにデータがありません。", MessageType.Warning);
            }
        }
    }
}