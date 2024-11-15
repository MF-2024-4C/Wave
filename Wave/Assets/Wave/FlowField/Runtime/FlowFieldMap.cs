using Quantum;
using UnityEngine;

namespace Wave.FlowField.Runtime
{
    [CreateAssetMenu(fileName = FileName, menuName = MenuName)]
    public class FlowFieldMap : ScriptableObject
    {
        private const string FileName = "FlowFieldMap";
        private const string MenuName = "FF/FlowFieldMap";
    }
}