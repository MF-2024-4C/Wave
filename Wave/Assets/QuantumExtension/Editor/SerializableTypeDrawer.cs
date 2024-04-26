using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Quantum.Editor
{
    public abstract class PropertyDrawerWithErrorHandling : PropertyDrawer
    {
        private SerializedProperty _currentProperty;

        private readonly Dictionary<string, Entry> _errors = new();
        private bool _hadError;
        private string _info;
        public float IconOffset;

        public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Debug.Assert(_currentProperty == null);

            var decoration = GetDecoration(property);

            if (decoration != null)
            {
                DrawDecoration(position, decoration.Value, label != GUIContent.none, true, false);
            }


            _currentProperty = property;
            _hadError = false;
            _info = null;

            EditorGUI.BeginChangeCheck();

            try
            {
                OnGUIInternal(position, property, label);
            }
            catch (ExitGUIException)
            {
                // pass through
            }
            catch (Exception ex)
            {
                SetError(ex.ToString());
            }
            finally
            {
                // if there was a change but no error clear
                if (EditorGUI.EndChangeCheck() && !_hadError)
                {
                    ClearError();
                }

                _currentProperty = null;
            }

            if (decoration != null)
            {
                DrawDecoration(position, decoration.Value, label != GUIContent.none, false, true);
            }
        }

        private void DrawDecoration(Rect position, (string, MessageType, bool) decoration, bool hasLabel,
            bool drawButton = true, bool drawIcon = true)
        {
            var iconPosition = position;
            iconPosition.height = UnityEditor.EditorGUIUtility.singleLineHeight;
            iconPosition.x -= IconOffset;
            QuantumEditorGUI.Decorate(iconPosition, decoration.Item1, decoration.Item2, hasLabel,
                drawButton: drawButton, drawBorder: decoration.Item3);
        }

        private (string, MessageType, bool)? GetDecoration(SerializedProperty property)
        {
            if (_errors.TryGetValue(property.propertyPath, out var error))
            {
                return (error.message, error.type, true);
            }

            if (_info != null)
            {
                return (_info, MessageType.Info, false);
            }

            return null;
        }

        protected abstract void OnGUIInternal(Rect position, SerializedProperty property, GUIContent label);

        protected void ClearError()
        {
            ClearError(_currentProperty);
        }

        protected void ClearError(SerializedProperty property)
        {
            _hadError = false;
            _errors.Remove(property.propertyPath);
        }

        protected void ClearErrorIfLostFocus()
        {
            if (GUIUtility.keyboardControl != EditorGUIUtility.LastControlID)
            {
                ClearError();
            }
        }

        protected void SetError(string error)
        {
            _hadError = true;
            _errors[_currentProperty.propertyPath] = new Entry
            {
                message = error,
                type = MessageType.Error
            };
        }

        protected void SetError(Exception error)
        {
            SetError(error.ToString());
        }

        protected void SetWarning(string warning)
        {
            if (_errors.TryGetValue(_currentProperty.propertyPath, out var entry) && entry.type == MessageType.Error)
            {
                return;
            }

            _errors[_currentProperty.propertyPath] = new Entry
            {
                message = warning,
                type = MessageType.Warning
            };
        }

        protected void SetInfo(string message)
        {
            if (_errors.TryGetValue(_currentProperty.propertyPath, out var entry) && entry.type == MessageType.Error ||
                entry.type == MessageType.Warning)
            {
                return;
            }

            _errors[_currentProperty.propertyPath] = new Entry
            {
                message = message,
                type = MessageType.Info
            };
        }

        private struct Entry
        {
            public string message;
            public MessageType type;
        }
    }

    [CustomPropertyDrawer(typeof(SerializableType<>))]
    [CustomPropertyDrawer(typeof(SerializableType))]
    public class SerializableTypeDrawer : PropertyDrawerWithErrorHandling
    {
        protected override void OnGUIInternal(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (SerializableTypeAttribute)attribute;

            SerializedProperty valueProperty;
            if (property.propertyType == SerializedPropertyType.String)
            {
                Debug.Assert(attr != null);
                valueProperty = property;
            }
            else
            {
                Debug.Assert(property.propertyType == SerializedPropertyType.Generic);
                valueProperty = property.FindPropertyRelativeOrThrow(nameof(SerializableType.AssemblyQualifiedName));
            }

            var assemblyQualifiedName = valueProperty.stringValue;

            var baseType = typeof(object);
            var leafType = fieldInfo.FieldType.GetUnityLeafType();
            if (leafType.IsGenericType && leafType.GetGenericTypeDefinition() == typeof(SerializableType<>))
            {
                baseType = leafType.GetGenericArguments()[0];
            }

            if (attr?.BaseType != null)
            {
                baseType = attr.BaseType;
            }

            position = EditorGUI.PrefixLabel(position, label);

            string content = "[None]";
            if (!string.IsNullOrEmpty(assemblyQualifiedName))
            {
                try
                {
                    var type = Type.GetType(assemblyQualifiedName, true);
                    content = type.FullName;
                }
                catch (Exception e)
                {
                    SetError(e);
                    content = assemblyQualifiedName;
                }
            }

            if (EditorGUI.DropdownButton(position, new GUIContent(content), FocusType.Keyboard))
            {
                ClearError();
                QuantumEditorGUI.DisplayTypePickerMenu(position, baseType, t =>
                {
                    string typeName = string.Empty;
                    if (t != null)
                    {
                        typeName = attr?.UseFullAssemblyQualifiedName == false
                            ? SerializableType.GetShortAssemblyQualifiedName(t)
                            : t.AssemblyQualifiedName;
                    }

                    valueProperty.stringValue = typeName;
                    valueProperty.serializedObject.ApplyModifiedProperties();
                });
            }
        }
    }
}