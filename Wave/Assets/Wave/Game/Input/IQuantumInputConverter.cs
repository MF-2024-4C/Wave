using System;
using Photon.Deterministic;
using Quantum;
using UnityEngine;

namespace Wave
{
    public abstract class QuantumInputConverter : ScriptableObject
    {
        public delegate void QuantumInputAction(int index,ReadOnlySpan<byte> bytes); 
        public event QuantumInputAction action;

        public virtual void InitializeElement(QuantumInputElement inputElement)
        {
            
        }

        protected virtual void OnAction(int index, ReadOnlySpan<byte> bytes)
        {
            action?.Invoke(index, bytes);
        }
    }
}