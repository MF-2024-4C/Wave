using System.Collections;
using TMPro;
using UnityEngine;

namespace Wave.Lobby.LoadingScreen
{
    public class LoadingScreen : MonoBehaviour
    {
        public static LoadingScreen Instance;

        [SerializeField] private GameObject _loading;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _loadingText;

        private bool _isShowing;

        private const float FadeTime = 0.1f;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void ShowLoading(string text)
        {
            if (_isShowing) return;
            _isShowing = true;
        
            StopAllCoroutines();
            _canvasGroup.alpha = 0;
        
            _loading.SetActive(true);
            _loadingText.text = text;
            StartCoroutine(FadeIn());
        }

        public void HideLoading()
        {
            if (!_isShowing) return;
            _isShowing = false;
        
            StopAllCoroutines();
            _canvasGroup.alpha = 1;
        
            StartCoroutine(FadeOut());
        }

        private IEnumerator FadeIn()
        {
            var time = 0f;
            while (time < FadeTime)
            {
                time += Time.deltaTime;
                _canvasGroup.alpha = time / FadeTime;
                yield return null;
            }
        }

        private IEnumerator FadeOut()
        {
            var time = 0f;
            while (time < FadeTime)
            {
                time += Time.deltaTime;
                _canvasGroup.alpha = 1 - time / FadeTime;
                yield return null;
            }

            _loading.SetActive(false);
        }
    }
}