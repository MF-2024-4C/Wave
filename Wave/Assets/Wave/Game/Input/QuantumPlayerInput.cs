using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Wave.Game
{
    public class QuantumPlayerInput : MonoBehaviour
    {
        [SerializeField] private int _hoge = 100;

        private Action<int> _onHogeChange;
        private Quantum.Input _input;

        public void Start()
        {
            Test();
        }

        public void Test()
        {
            Guid.NewGuid().ToByteArray();
            var serializeInput = new Int32[168 / 4];
            var fireButton = new Quantum.Button { };
            fireButton = true;
            serializeInput.SetValue(fireButton, 44 / 4);
            var input = MemoryMarshal.Cast<Int32, Quantum.Input>(serializeInput);
            Debug.Log("Inputting Test");
            Debug.Log($"Fire:{input[0].Fire.IsDown},Test:{input[0].Jump.IsDown}");
        }
    }
}