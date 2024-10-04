using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Wave.NavigationMarker.InstructionMessage;
using static UnityEngine.Screen;

namespace Wave.NavigationMarker.NavigationMarker
{
    public class NavigationMarkerViewer : MonoBehaviour
    {
        [SerializeField, Header("スペースキーで次のマーカーを表示する")]
        private bool _debugMode;

        [SerializeField, Header("開始時に一つ目のマーカーを表示する")]
        private bool _showMarkerOnStart;

        [SerializeField] private Image _markerImage, _cursorImage, _markerCanvasGroup;
        private RectTransform _markerRectTransform, _cursorRectTransform;
        [SerializeField] private Sprite _markerSprite, _outOfScreenMarkerSprite;

        [SerializeField] private NavigationManager _navigationManager;
        private int _currentNavigationPointIndex = -1;
        private Vector3 _currentNavigationPointPosition;

        private const float MarkerScaleDuration = 0.5f;
        private bool _isInitializing, _markerAnimationPlaying;

        [SerializeField] private InstructionMessageViewer _instructionMessageViewer;

        private void Start()
        {
            _markerImage.TryGetComponent(out _markerRectTransform);
            _cursorImage.TryGetComponent(out _cursorRectTransform);

            if (_showMarkerOnStart)
            {
                ShowMarker();
                SetMarkerPositionByIndex(0);
            }
            else HideMarker();
        }

        private void Update()
        {
            SetMarkerPosition(_currentNavigationPointPosition);

            if (_debugMode && Application.isEditor && Input.GetKeyDown(KeyCode.Space))
                SetNextMarkerPosition();
        }

        public void SetNextMarkerPosition()
        {
            _currentNavigationPointIndex++;
            if (_currentNavigationPointIndex >= _navigationManager._navigationPoints.Count)
                return;

            SetMarkerPositionByIndex(_currentNavigationPointIndex);
        }

        public void SetMarkerPositionByIndex(int index)
        {
            if (index < 0 || index >= _navigationManager._navigationPoints.Count)
                return;

            ShowMarker();

            PlayMarkerAnimation();

            _currentNavigationPointIndex = index;

            SetMarkerData(_navigationManager._navigationPoints[_currentNavigationPointIndex]);
        }

        private void SetMarkerData(NavigationManager.NavigationPoint point)
        {
            _currentNavigationPointPosition = point.Position;

            var data = point.markerData;
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<NavigationMarkerData>();
                data.Color = Color.white;
                data.MainText = "Head to your destination.";
                data.SubText = "";
                data.Icon = _markerSprite;
            }

            if (data.Icon)
                _markerImage.sprite = data.Icon;
            else
            {
                _markerImage.sprite = _markerSprite;
                data.Icon = _markerSprite;
            }

            _markerImage.color = data.Color;

            _instructionMessageViewer.ShowMessage(data);
        }


        private Sequence _markerAnimationSequence;

        private void PlayMarkerAnimation()
        {
            _markerAnimationSequence?.Complete();
            _isInitializing = true;
            _markerAnimationPlaying = true;
            _cursorImage.gameObject.SetActive(false);
            _markerRectTransform.localScale = Vector3.one * 20.0f;
            _markerRectTransform.position = new Vector3(width / 2, height / 2, 0);

            _markerAnimationSequence = DOTween.Sequence();
            _markerAnimationSequence.Append(_markerRectTransform.DOScale(Vector3.one * 20.0f, 0))
                .Append(_markerCanvasGroup.DOFade(0.1f, 0))
                .Append(_markerRectTransform.DOScale(Vector3.one * 10.0f, MarkerScaleDuration).SetEase(Ease.InExpo))
                .AppendCallback(() => _isInitializing = false)
                .Append(_markerCanvasGroup.DOFade(1, MarkerScaleDuration))
                .Join(_markerRectTransform.DOScale(Vector3.one, MarkerScaleDuration).SetEase(Ease.OutExpo));

            _markerAnimationSequence.OnComplete(() => _markerAnimationPlaying = false);
        }

        public void ShowMarker()
        {
            _markerImage.gameObject.SetActive(true);
        }

        public void HideMarker()
        {
            _markerImage.gameObject.SetActive(false);
        }

        private void SetMarkerPosition(Vector3 worldPosition)
        {
            if (_isInitializing) return;

            var camera = Camera.main;
            if (camera == null) return;

            var viewportPosition = camera.WorldToViewportPoint(worldPosition);
            if (viewportPosition.z < 0 || viewportPosition.x < 0 || viewportPosition.x > 1 || viewportPosition.y < 0 ||
                viewportPosition.y > 1)
            {
                SetOutOfScreenMarkerPosition(worldPosition, camera, viewportPosition);
            }
            else
            {
                SetInScreenMarkerPosition(worldPosition, camera);
            }
        }

        private void SetOutOfScreenMarkerPosition(Vector3 worldPosition, Camera camera, Vector3 viewportPosition)
        {
            var screenPosition = camera.WorldToScreenPoint(worldPosition);
            screenPosition = viewportPosition.z < 0 ? -screenPosition : screenPosition;
            var clampedPosition = ClampToScreenBounds(screenPosition);

            _markerRectTransform.position = _markerAnimationPlaying
                ? Vector3.Lerp(_markerRectTransform.position, clampedPosition, Time.deltaTime * 10)
                : clampedPosition;

            _cursorImage.gameObject.SetActive(true);
            SetMarkerRotation(clampedPosition);
        }

        private void SetInScreenMarkerPosition(Vector3 worldPosition, Camera camera)
        {
            var screenPosition = camera.WorldToScreenPoint(worldPosition);

            _markerRectTransform.position = _markerAnimationPlaying
                ? Vector3.Lerp(_markerRectTransform.position, screenPosition, Time.deltaTime * 10)
                : screenPosition;

            _cursorImage.gameObject.SetActive(false);
        }

        private Vector3 ClampToScreenBounds(Vector3 screenPosition)
        {
            const int offset = 50;
            return new Vector3(
                Mathf.Clamp(screenPosition.x, offset, width - offset),
                Mathf.Clamp(screenPosition.y, offset, height - offset),
                screenPosition.z
            );
        }

        private void SetMarkerRotation(Vector3 screenPosition)
        {
            var center = new Vector3(width / 2, height / 2, 0);
            var direction = (screenPosition - center).normalized;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _cursorRectTransform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }
}