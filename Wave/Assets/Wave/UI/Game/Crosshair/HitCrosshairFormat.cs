using UnityEngine;

namespace Wave.UI.Game
{
    [CreateAssetMenu(fileName = "HitCrosshairFormat", menuName = "Wave/UI/HitCrosshairFormat", order = 0)]
    public class HitCrosshairFormat : ScriptableObject
    {
        [SerializeField, Range(0.1f, 3)] private float _duration = 1;
        public float Duration => _duration;

        [SerializeField, Range(5, 50)] public float _increaseAmount = 25;
        public float IncreaseAmount => _increaseAmount;

        [SerializeField] private Color _hitMarkerColor = Color.white;
        public Color HitMarkerColor => _hitMarkerColor;

        [SerializeField] private AnimationCurve _hitScaleCurve;
        public AnimationCurve HitScaleCurve => _hitScaleCurve;

        [SerializeField] private AnimationCurve _hitAlphaCurve;
        public AnimationCurve HitAlphaCurve => _hitAlphaCurve;
    }
}