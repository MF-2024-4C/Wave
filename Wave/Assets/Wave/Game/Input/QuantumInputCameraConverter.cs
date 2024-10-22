using UnityEngine;

namespace Wave.Game
{
    [CreateAssetMenu(fileName = FileName, menuName = MenuName, order = 0)]
    public class QuantumInputCameraConverter : QuantumInputConverter
    {
        private const string FileName = "QuantumInputCameraConverter";
        private const string MenuName = "Wave/Input/QuantumInputCameraConverter";
        
        public override void InitializeElement(QuantumInputElement inputElement)
        {

        }
    }
}