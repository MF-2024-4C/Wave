using UnityEngine;

namespace Wave.UI.Game
{
    [CreateAssetMenu(fileName = "CrosshairFormat", menuName = "Wave/UI/CrosshairFormat", order = 0)]
    public class CrosshairFormat : ScriptableObject
    {
        [SerializeField] private float _onFireScaleAmount = 75;
        [SerializeField] private float _onAimScaleAmount = 35;
        
        public float OnFireScaleAmount => _onFireScaleAmount;
        public float OnAimScaleAmount => _onAimScaleAmount;
    }
}