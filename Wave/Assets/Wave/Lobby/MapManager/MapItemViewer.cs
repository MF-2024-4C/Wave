using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wave.Lobby.MapManager;

public class MapItemViewer : MonoBehaviour
{
    [SerializeField] private MapItemManager _mapItemPrefab;
    [SerializeField] private Transform _content;

    [SerializeField] private MapManager _mapManager;

    private void OnEnable()
    {
        ViewMapItems();
    }

    private void ViewMapItems()
    {
        ClearAllMapItems();

        foreach (var map in _mapManager.Maps)
        {
            AddMapItem(map);
        }
    }

    private void ClearAllMapItems()
    {
        foreach (Transform child in _content)
        {
            Destroy(child.gameObject);
        }
    }

    private void AddMapItem(MapInfo map)
    {
        var mapItem = Instantiate(_mapItemPrefab, _content);
        mapItem.SetMapInfo(map);
    }
}