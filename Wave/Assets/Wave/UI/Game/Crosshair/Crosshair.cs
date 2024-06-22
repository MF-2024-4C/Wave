using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Wave.UI.Game
{
    public class Crosshair : MonoBehaviour
    {
        [Header("Settings")] [Range(1, 10)] public float _scaleLerp = 5;
        [Range(0.01f, 1)] public float _onFireScaleRate = 0.1f;
        [SerializeField] private CrosshairFormat _crosshairFormat;
        [SerializeField] private HitCrosshairFormat _hitCrosshairFormat;


        [Header("References")] [SerializeField]
        private RectTransform _crosshairContainer;
        [SerializeField] private RectTransform _hitMarkerRoot;


        private Vector2 _initSizeDelta;
        private Vector2 _extraScale = Vector2.one * 8;
        private float _lastTimeFire;

        private float _hitDuration;
        private CanvasGroup _hitAlpha;
        private Vector2 _defaultHitSize;


        public void OnFire()
        {
            if (Time.time < _lastTimeFire)
                return;

            var size = new Vector2(_crosshairFormat.OnFireScaleAmount, _crosshairFormat.OnFireScaleAmount);
            _crosshairContainer.sizeDelta = size;
            _lastTimeFire = Time.time + _onFireScaleRate;
        }

        public void OnHit()
        {
            if (_hitMarkerRoot == null)
                return;


            StopCoroutine(nameof(OnHitMarker));
            StartCoroutine(nameof(OnHitMarker));
        }


        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            _initSizeDelta = _crosshairContainer.sizeDelta;

            if (_hitMarkerRoot != null)
            {
                _hitAlpha = _hitMarkerRoot.GetComponent<CanvasGroup>();
                _defaultHitSize = _hitMarkerRoot.sizeDelta;
                if (_hitAlpha != null)
                {
                    _hitAlpha.alpha = 0;
                }

                var hmg = _hitMarkerRoot.GetComponentsInChildren<Graphic>();
                foreach (var g in hmg)
                {
                    g.color = _hitCrosshairFormat.HitMarkerColor;
                }
            }
        }

        private void Update()
        {
            ScaleContent();
        }

        private void ScaleContent()
        {
            var target = _initSizeDelta + _extraScale;
            _crosshairContainer.sizeDelta =
                Vector2.Lerp(_crosshairContainer.sizeDelta, target, Time.deltaTime * _scaleLerp);
        }

        private IEnumerator OnHitMarker()
        {
            _hitDuration = 0;
            _hitMarkerRoot.sizeDelta = _defaultHitSize;
            var sizeTarget = new Vector2(_hitCrosshairFormat.IncreaseAmount, _hitCrosshairFormat.IncreaseAmount);
            _crosshairContainer.gameObject.SetActive(false);

            while (_hitDuration < 1)
            {
                _hitDuration += Time.unscaledDeltaTime / _hitCrosshairFormat.Duration;
                _hitMarkerRoot.sizeDelta = Vector2.Lerp(_defaultHitSize, sizeTarget,
                    _hitCrosshairFormat.HitScaleCurve.Evaluate(_hitDuration));
                if (_hitAlpha != null)
                {
                    _hitAlpha.alpha = _hitCrosshairFormat.HitAlphaCurve.Evaluate(_hitDuration);
                }

                yield return null;
            }
            _crosshairContainer.gameObject.SetActive(true);
        }
    }
}