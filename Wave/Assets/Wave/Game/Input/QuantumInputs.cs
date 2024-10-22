using System;
using Photon.Deterministic;
using Quantum;
using UnityEngine;

namespace Wave
{
    public enum QuantumInputType
    {
        Boolean,
        Button,
        Vector2,
        Vector3,
    }

    public static class QuantumInputHelper
    {
        public static int GetSize(QuantumInputType type)
        {
            return type switch
            {
                QuantumInputType.Boolean => QBoolean.SIZE,
                QuantumInputType.Button => Button.SIZE,
                QuantumInputType.Vector2 => FPVector2.SIZE,
                QuantumInputType.Vector3 => FPVector3.SIZE,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
    
    [Serializable]
    public struct QuantumInputElement
    {
        [SerializeField] public int Offset;
        public QuantumInputType Type;
        public int InputID;
        public string InputName;
    }
}