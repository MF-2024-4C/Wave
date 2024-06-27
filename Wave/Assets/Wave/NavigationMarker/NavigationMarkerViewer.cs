using UnityEngine;
using Wave.NavimationMarker;

public class NavigationMarkerViewer : MonoBehaviour
{
    [SerializeField] private Camera _overlayCamera, _mainCamera;
    [SerializeField] private RectTransform _markerImage;

    [SerializeField] private NavigationManager _navigationManager;
    private int _currentNavigationPointIndex = 0;
    private Vector3 _currentNavigationPointPosition;
    
    private const float MarkerScale = 0.1f;
    private const float MarkerScaleDuration = 1.0f;


    private void Start()
    {
        ShowMarker();
    }
    
    private void Update()
    {
        //オーバーレイカメラの座標をメインカメラと同期
        _overlayCamera.transform.position = _mainCamera.transform.position;
        SetMarkerPosition(_currentNavigationPointPosition);
        
        if ( Input.GetKeyDown(KeyCode.Space) )
            SetNextMarkerPosition();
    }

    public void SetNextMarkerPosition()
    {
        _currentNavigationPointIndex++;
        if (_currentNavigationPointIndex >= _navigationManager._navigationPoints.Count)
            return;
        
        
        _currentNavigationPointPosition = _navigationManager._navigationPoints[_currentNavigationPointIndex].Position;
    }
    
    public void SetMarkerPositionByIndex(int index)
    {
        if (index < 0 || index >= _navigationManager._navigationPoints.Count)
            return;

        _currentNavigationPointIndex = index;
        _currentNavigationPointPosition = _navigationManager._navigationPoints[_currentNavigationPointIndex].Position;
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
        var screenPosition = _mainCamera.WorldToScreenPoint(worldPosition);
        _markerImage.position = screenPosition;
    }
}