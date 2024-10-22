using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Photon.Deterministic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Wave.Game;
using Button = UnityEngine.UIElements.Button;
using Input = Quantum.Input;

namespace Wave.Editor
{
    [CustomEditor(typeof(QuantumPlayerInputSystem))]
    public class QuantumPlayerInputSystemEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();

            // SerializedObjectを取得して、プロパティを操作できるようにする
            SerializedProperty quantumInputsProperty = serializedObject.FindProperty("_quantumInputs._quantumInput");
            SerializedProperty inputActionsProperty = serializedObject.FindProperty("_quantumInputs._inputAction");
            SerializedProperty ContextPropery = serializedObject.FindProperty("_context");
            
            var contextField = new PropertyField(ContextPropery, "Context");
            container.Add(contextField);
            
            // 配列全体のDropdownButtonを追加して要素を表示する
            Foldout quantumInputsFoldout = new Foldout() { text = "Quantum Inputs", value = true };
            container.Add(quantumInputsFoldout);

            Type inputType = typeof(Input);
            FieldInfo[] fields = inputType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            quantumInputsProperty.arraySize = fields.Length;
            inputActionsProperty.arraySize = fields.Length;
            
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                SerializedProperty element = quantumInputsProperty.GetArrayElementAtIndex(i);
                SerializedProperty offsetProperty = element.FindPropertyRelative("Offset");
                SerializedProperty typeProperty = element.FindPropertyRelative("Type");
                SerializedProperty inputIdProperty = element.FindPropertyRelative("InputID");
                SerializedProperty inputNameProperty = element.FindPropertyRelative("InputName");

                // 各要素のフィールド名を表示するDropdownButtonを作成
                var elementContainer = new VisualElement();
                var elementFoldout = new Foldout() { text = field.Name, value = false };
                elementContainer.Add(elementFoldout);

                if (offsetProperty != null)
                {
                    FieldOffsetAttribute offsetAttr = field.GetCustomAttribute<FieldOffsetAttribute>();
                    offsetProperty.intValue = offsetAttr != null ? offsetAttr.Value : 0;
                }

                if (typeProperty != null)
                {
                    typeProperty.enumValueIndex = (int)GetQuantumInputTypeFromFieldType(field.FieldType);
                }

                if (inputIdProperty != null)
                {
                    inputIdProperty.intValue = i;
                }
                
                if (inputNameProperty != null)
                {
                    inputNameProperty.stringValue = field.Name;
                }

                var horizontalContainer1 = new VisualElement();
                horizontalContainer1.style.flexDirection = FlexDirection.Row;
                horizontalContainer1.style.justifyContent = Justify.FlexStart;

                var offsetLabel = new Label("Offset:") { style = { minWidth = 50, unityTextAlign = TextAnchor.MiddleLeft } };
                var offsetValue = new Label(offsetProperty.intValue.ToString()) { style = { minWidth = 50, unityTextAlign = TextAnchor.MiddleLeft } };
                horizontalContainer1.Add(offsetLabel);
                horizontalContainer1.Add(offsetValue);

                var typeLabel = new Label("Type:") { style = { minWidth = 50, unityTextAlign = TextAnchor.MiddleLeft } };
                var typeValue = new Label(typeProperty.enumDisplayNames[typeProperty.enumValueIndex]) { style = { minWidth = 100, unityTextAlign = TextAnchor.MiddleLeft } };
                horizontalContainer1.Add(typeLabel);
                horizontalContainer1.Add(typeValue);
                
                elementFoldout.Add(horizontalContainer1);
                
                var horizontalContainer2 = new VisualElement();
                horizontalContainer2.style.flexDirection = FlexDirection.Row;
                horizontalContainer2.style.justifyContent = Justify.FlexStart;

                // Input IDのラベルと値を編集可能に変更
                var inputIdLabel = new Label("Input ID:") { style = { minWidth = 50, unityTextAlign = TextAnchor.MiddleLeft } };
                var inputIdField = new IntegerField() { value = inputIdProperty.intValue, style = { minWidth = 50 } };
                inputIdField.BindProperty(inputIdProperty);
                
                horizontalContainer2.Add(inputIdLabel);
                horizontalContainer2.Add(inputIdField);

                // Input Nameのラベルと値を編集可能に変更
                var inputNameLabel = new Label("Input Name:") { style = { minWidth = 50, unityTextAlign = TextAnchor.MiddleLeft } };
                var inputNameField = new TextField() { value = inputNameProperty.stringValue, style = { minWidth = 100 } };
                inputNameField.BindProperty(inputNameProperty);
                
                horizontalContainer2.Add(inputNameLabel);
                horizontalContainer2.Add(inputNameField);

                elementFoldout.Add(horizontalContainer2);

                // InputActionReferenceを追加して表示
                if (i < inputActionsProperty.arraySize)
                {
                    SerializedProperty actionElement = inputActionsProperty.GetArrayElementAtIndex(i);
                    elementFoldout.Add(new PropertyField(actionElement, "QuantumInputConverter"));
                }

                quantumInputsFoldout.Add(elementContainer);
            }

            // 追加のUI要素を配置する場合はここで作成することもできます
            // 例えばボタンを追加して配列の要素を増やしたり、減らしたりする

            var addButton = new Button(() =>
            {
                UpdateQuantumInputsArray(quantumInputsProperty);
                serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
            })
            {
                text = "Update Quantum Inputs"
            };
            container.Add(addButton);

            return container;
        }

        private void UpdateQuantumInputsArray(SerializedProperty quantumInputsProperty)
        {
            Type inputType = typeof(Input);
            FieldInfo[] fields = inputType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            quantumInputsProperty.arraySize = fields.Length;

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                SerializedProperty element = quantumInputsProperty.GetArrayElementAtIndex(i);
                SerializedProperty offsetProperty = element.FindPropertyRelative("Offset");
                SerializedProperty typeProperty = element.FindPropertyRelative("Type");
                SerializedProperty inputIdProperty = element.FindPropertyRelative("InputID");
                SerializedProperty inputNameProperty = element.FindPropertyRelative("InputName");

                if (offsetProperty != null)
                {
                    FieldOffsetAttribute offsetAttr = field.GetCustomAttribute<FieldOffsetAttribute>();
                    offsetProperty.intValue = offsetAttr != null ? offsetAttr.Value : 0;
                }

                if (typeProperty != null)
                {
                    typeProperty.enumValueIndex = (int)GetQuantumInputTypeFromFieldType(field.FieldType);
                }

                if (inputIdProperty != null)
                {
                    inputIdProperty.intValue = i;
                }
                
                if (inputNameProperty != null)
                {
                    inputNameProperty.stringValue = field.Name;
                }
            }
        }

        private QuantumInputType GetQuantumInputTypeFromFieldType(Type fieldType)
        {
            return fieldType switch
            {
                not null when fieldType == typeof(Quantum.Button) => QuantumInputType.Button,
                not null when fieldType == typeof(Quantum.QBoolean) => QuantumInputType.Boolean,
                not null when fieldType == typeof(FPVector2) => QuantumInputType.Vector2,
                not null when fieldType == typeof(FPVector3) => QuantumInputType.Vector3,
                _ => throw new ArgumentOutOfRangeException(nameof(fieldType), fieldType,
                    $"Unsupported field type: {fieldType.FullName}")
            };
        }
    }
}