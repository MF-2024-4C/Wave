using UnityEngine;
using Wave.NavimationMarker;
using DG.Tweening;

public class NavigationMarkerViewer : MonoBehaviour
{
    [SerializeField] private RectTransform _markerImage;

    [SerializeField] private NavigationManager _navigationManager;
    private int _currentNavigationPointIndex = 0;
    private Vector3 _currentNavigationPointPosition;

    private const float MarkerScaleDuration = 1.0f;


    private void Start()
    {
        ShowMarker();
    }

    private void Update()
    {
        SetMarkerPosition(_currentNavigationPointPosition);

        if (Input.GetKeyDown(KeyCode.Space))
            SetNextMarkerPosition();
    }

    public void SetNextMarkerPosition()
    {
        _currentNavigationPointIndex++;
        if (_currentNavigationPointIndex >= _navigationManager._navigationPoints.Count)
            return;

        PlayMarkerAnimation();

        _currentNavigationPointPosition = _navigationManager._navigationPoints[_currentNavigationPointIndex].Position;
    }

    public void SetMarkerPositionByIndex(int index)
    {
        if (index < 0 || index >= _navigationManager._navigationPoints.Count)
            return;

        PlayMarkerAnimation();

        _currentNavigationPointIndex = index;
        _currentNavigationPointPosition = _navigationManager._navigationPoints[_currentNavigationPointIndex].Position;
    }

    private void PlayMarkerAnimation()
    {
        _markerImage.DOKill();

        _markerImage.localScale = Vector3.one * 20.0f;
        _markerImage.DOScale(Vector3.one, MarkerScaleDuration).SetEase(Ease.OutQuad);
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
        var screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        _markerImage.position = screenPosition;
    }
}