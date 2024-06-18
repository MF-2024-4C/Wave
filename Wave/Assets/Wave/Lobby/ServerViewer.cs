using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class ServerViewer : MonoBehaviour
{
    [SerializeField] private Transform _content;
    [SerializeField] private ServerItem _serverItemPrefab;

    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Clear();
        foreach (var roomInfo in roomList)
        {
            AddServerItem(roomInfo);
        }
    }

    private void Clear()
    {
        foreach (Transform child in _content)
        {
            Destroy(child.gameObject);
        }
    }

    private void AddServerItem(RoomInfo roomInfo)
    {
        var serverItem = Instantiate(_serverItemPrefab, _content);
        serverItem.SetRoomInfo(roomInfo);
    }
}