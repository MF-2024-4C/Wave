using System.Collections.Generic;
using System.Linq;
using Photon.Realtime;
using UnityEngine;

namespace Wave.Lobby
{
    public class RoomViewer : MonoBehaviour
    {
        [SerializeField] private Transform _content;
        [SerializeField] private RoomItem _roomItemPrefab;

        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Clear();
            foreach (var roomInfo in roomList)
            {
                AddRoomItem(roomInfo);
            }
        }

        private void Clear()
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