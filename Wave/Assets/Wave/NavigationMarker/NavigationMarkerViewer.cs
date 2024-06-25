using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wave.NavimationMarker;

public class NavigationMarkerViewer : MonoBehaviour
{
    [SerializeField] private NavigationManager _navigationManager;

    [SerializeField] private bool _showFirstNavigationMarker;

    [SerializeField] private GameObject _navigationMarkerObject;
    private int _currentNavigationMarkerIndex = 0;

    [SerializeField] private Transform _navigationMarkerParent;

    private void Start()
    {
        if (_showFirstNavigationMarker) ShowNextNavigationMarker();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ShowNextNavigationMarker();
    }

    public void ShowNextNavigationMarker()
    {
        if (_currentNavigationMarkerIndex >= _navigationManager._navigationPoints.Count) return;
        ClearNavigationMarkers();
        var navigationPoint = _navigationManager._navigationPoints[_currentNavigationMarkerIndex];
        Instantiate(_navigationMarkerObject, navigationPoint.Position, Quaternion.identity, _navigationMarkerParent);
        _currentNavigationMarkerIndex++;
    }

    public void ShowNavigationMarkerByIndex(int index)
    {
        if (index >= _navigationManager._navigationPoints.Count) return;
        ClearNavigationMarkers();
        var navigationPoint = _navigationManager._navigationPoints[index];
        Instantiate(_navigationMarkerObject, navigationPoint.Position, Quaternion.identity, _navigationMarkerParent);
    }

    public void HideNavigationMarkers()
    {
        ClearNavigationMarkers();
    }

    public void ShowAllNavigationMarkers()
    {
        ClearNavigationMarkers();
        ShowNavigationMarkerByIndex(_currentNavigationMarkerIndex);
    }

    public void ClearNavigationMarkers()
    {
        foreach (Transform child in _navigationMarkerParent)
            Destroy(child.gameObject);
    }
}