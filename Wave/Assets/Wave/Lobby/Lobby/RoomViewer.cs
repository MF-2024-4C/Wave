using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

namespace Wave.Lobby.Lobby
{
    public class RoomViewer : MonoBehaviour
    {
        [SerializeField] private Transform _content;
        [SerializeField] private RoomItem _roomItemPrefab;

        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            ClearAllRoomItems();
            foreach (var roomInfo in roomList)
            {
                if (roomInfo.RemovedFromList) continue;
                if (!roomInfo.IsVisible) continue;
                if (roomInfo.PlayerCount == 0) continue;
                
                AddRoomItem(roomInfo);
            }
        }

        public void ClearAllRoomItems()
        {
            foreach (Transform child in _content)
            {
                Destroy(child.gameObject);
            }
        }

        private void AddRoomItem(RoomInfo roomInfo)
        {
            var roomItem = Instantiate(_roomItemPrefab, _content);
            roomItem.SetRoomInfo(roomInfo);
        }
    }
}