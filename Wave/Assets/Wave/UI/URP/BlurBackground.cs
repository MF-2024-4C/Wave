namespace Wave.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Image))]
    [ExecuteAlways]
    public class BlurBackground : MonoBehaviour
    {
        private const string BlurBackgroundUIShaderPath = "BlurBackgroundUI";
        private static readonly int ShaderPropertyIdBlurBlendRate = Shader.PropertyToID("_BlurBlendRate");

        [SerializeField] [Range(0.0f, 1.0f)] private float _blurBlendRate = 1.0f;

        private Material _material;

#if UNITY_EDITOR
        private void Update()
        {
            BlendRate = _blurBlendRate;
        }
#endif

        private void OnEnable()
        {
            SetImageMaterial();
        }

        private void OnDestroy()
        {
            if (_material == null) return;
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                DestroyImmediate(_material);
            }
            else
            {
                Destroy(_material);
            }
            #else
            Destroy(_material);
            #endif
            _material = null;
        }

        public float BlendRate
        {
            get => _blurBlendRate;
            set
            {
                _blurBlendRate = Mathf.Clamp01(value);

                if (_material != null)
                {
                    _material.SetFloat(ShaderPropertyIdBlurBlendRate, _blurBlendRate);
                }
            }
        }

        private void SetImageMaterial()
        {
            if (_material == null)
            {
                _material = new Material(Shader.Find(BlurBackgroundUIShaderPath));
            }

            _material.SetFloat(ShaderPropertyIdBlurBlendRate, _blurBlendRate);

            var targetImage = GetComponent<Image>();
            targetImage.material = _material;
        }
    }
}