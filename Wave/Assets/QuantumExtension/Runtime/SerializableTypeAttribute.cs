using UnityEngine;

namespace Quantum
{
    public class SerializableTypeAttribute : PropertyAttribute
    {
        public System.Type BaseType { get; set; }

        public bool UseFullAssemblyQualifiedName { get; set; }
    }
}