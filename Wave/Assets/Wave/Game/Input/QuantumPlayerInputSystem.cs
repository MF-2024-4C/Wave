using System;
using System.Runtime.InteropServices;
using Photon.Deterministic;
using Quantum;
using UnityEngine;
using Input = Quantum.Input;

namespace Wave.Game
{
    [Serializable]
    public class QuantumInputSystem
    {
        [SerializeField] private QuantumInputElement[] _quantumInput;
        [SerializeField] private QuantumInputConverter[] _inputAction;

        public QuantumInputElement[] QuantumInput => _quantumInput;
        public QuantumInputConverter[] InputAction => _inputAction;
    }

    public class QuantumPlayerInputSystem : MonoBehaviour
    {
        [SerializeField] private QuantumInputSystem _quantumInputs;
        private byte[] _inputData;
        private DispatcherSubscription _subscription;

        private void OnEnable()
        {
            _subscription = QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
        }

        private void OnDisable()
        {
            QuantumCallback.Unsubscribe(_subscription);
        }

        private void PollInput(CallbackPollInput callback)
        {
            var input = MemoryMarshal.Read<Input>(_inputData.AsSpan());
            callback.SetInput(input, DeterministicInputFlags.Repeatable);
        }

        private void Awake()
        {
            _inputData = new byte[Input.SIZE];
            for (var i = 0; i < _quantumInputs.InputAction.Length; i++)
            {
                if (_quantumInputs.InputAction[i] == null)
                {
                    continue;
                }

                _quantumInputs.InputAction[i].InitializeElement(_quantumInputs.QuantumInput[i]);
                _quantumInputs.InputAction[i].action += OnInput;
            }
        }

        private void OnInput(int index, ReadOnlySpan<byte> bytes)
        {
            bytes.CopyTo(_inputData.AsSpan(_quantumInputs.QuantumInput[index].Offset));
        }
    }
}