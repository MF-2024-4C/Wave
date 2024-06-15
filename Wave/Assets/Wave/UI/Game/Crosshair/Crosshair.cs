using Quantum;
using UnityEngine;

namespace Wave.UI.Game
{
    public class Crosshair : MonoBehaviour
    {
        [Header("Settings")] [Range(1, 10)] public float _scaleLerp = 5;
        [Range(0.01f, 1)] public float _onFireScaleRate = 0.1f;
        [SerializeField] private CrosshairFormat _crosshairFormat = null;

        [Header("References")] [SerializeField]
        private RectTransform _crosshairContainer = null;

        [SerializeField] private RectTransform _hitMarkerRoot = null;


        private Vector2 _initSizeDelta;
        private Vector2 _extraScale = Vector2.one * 8;

        private float _lastTimeFire;

        public void OnFire()
        {
            if (Time.time < _lastTimeFire)
                return;

            var size = new Vector2(_crosshairFormat.OnFireScaleAmount, _crosshairFormat.OnFireScaleAmount);
            _crosshairContainer.sizeDelta = size;
            _lastTimeFire = Time.time + _onFireScaleRate;
        }


        private void Awake()
        {
            Initialize();
        }

        private void OnFireEvent(EventFire e)
        {
            OnFire();
        }

        private void Initialize()
        {
            _initSizeDelta = _crosshairContainer.sizeDelta;
        }

        private void Update()
        {
            ScaleContent();
            
            if(UnityEngine.Input.GetMouseButton(0))
                OnFire();
        }

        private void ScaleContent()
        {
            var target = _initSizeDelta + _extraScale;
            _crosshairContainer.sizeDelta =
                Vector2.Lerp(_crosshairContainer.sizeDelta, target, Time.deltaTime * _scaleLerp);
        }
    }
}