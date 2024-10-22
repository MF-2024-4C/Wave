using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Photon.Deterministic;
using Quantum;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using System.Reflection;
#endif

namespace Wave.Game
{
    [CreateAssetMenu(fileName = FileName, menuName = MenuName, order = 0)]
    public class QuantumInputSystemConverter : QuantumInputConverter, ISerializationCallbackReceiver
    {
        private const string FileName = "QuantumInputSystemConverterMap";
        private const string MenuName = "Wave/Input/QuantumInputSystemConverter";

        [Serializable]
        public struct QuantumInputSystemElement
        {
            public string Name;
            public InputActionReference ActionReference;
        }

        [SerializeField] private QuantumInputSystemElement[] _inputSystemElements;
        private bool _initialized;

        private Dictionary<string, InputActionReference> _inputSystemMap = new();

        public override void InitializeElement(QuantumInputElement inputElement)
        {
            if (!_initialized)
            {
                _initialized = true;
                foreach (var element in _inputSystemElements)
                {
                    _inputSystemMap[element.Name] = element.ActionReference;
                }
            }

            if (_inputSystemMap.TryGetValue(inputElement.InputName, out var actionReference))
            {
                actionReference.action.Enable();
                actionReference.action.performed += ctx => OnInput(inputElement, ctx);
                if (inputElement.Type == QuantumInputType.Button)
                {
                    actionReference.action.canceled += ctx => OnInput(inputElement, ctx);
                }
            }
        }

        private void OnInput(QuantumInputElement inputElement, InputAction.CallbackContext ctx)
        {
            var value = ctx.phase switch
            {
                InputActionPhase.Performed => true,
                InputActionPhase.Canceled => false,
                _ => default
            };

            var size = QuantumInputHelper.GetSize(inputElement.Type);
            Span<byte> bytes = stackalloc byte[size];
            switch (inputElement.Type)
            {
                case QuantumInputType.Boolean:
                    QBoolean qBoolean = value;
                    MemoryMarshal.Write(bytes, ref qBoolean);
                    break;
                case QuantumInputType.Button:
                    Button qButton = value;
                    MemoryMarshal.Write(bytes, ref qButton);
                    break;
                case QuantumInputType.Vector2:
                    FPVector2 qVector2 = ctx.ReadValue<Vector2>().ToFPVector2();
                    MemoryMarshal.Write(bytes, ref qVector2);
                    break;
                case QuantumInputType.Vector3:
                    FPVector3 qVector3 = ctx.ReadValue<Vector3>().ToFPVector3();
                    MemoryMarshal.Write(bytes, ref qVector3);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            OnAction(inputElement.InputID, bytes);
        }

        public void OnBeforeSerialize()
        {
            _initialized = false;
            _inputSystemMap = new Dictionary<string, InputActionReference>();
        }

        public void OnAfterDeserialize()
        {
        }

        #if UNITY_EDITOR
        private void Reset()
        {
            var inputType = typeof(Quantum.Input);
            var fields = inputType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            _inputSystemElements = new QuantumInputSystemElement[fields.Length];
            for (var i = 0; i < fields.Length; i++)
            {
                _inputSystemElements[i].Name = fields[i].Name;
            }
        }
        #endif
    }
}