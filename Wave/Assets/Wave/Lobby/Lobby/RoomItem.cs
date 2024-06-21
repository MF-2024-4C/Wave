using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Wave.Lobby.Lobby
{
    public class RoomItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _roomName;
        [SerializeField] private TextMeshProUGUI _playerCount;
        private RoomInfo _roomInfo;

        public void SetRoomInfo(RoomInfo roomInfo)
        {
            Debug.Log(roomInfo.CustomProperties["ROOM-NAME"].ToString());
            _roomName.text = roomInfo.CustomProperties["ROOM-NAME"].ToString();
            _playerCount.text = $"{roomInfo.PlayerCount}/{roomInfo.MaxPlayers}";

            _roomInfo = roomInfo;
        }

        public void OnClick()
        {
            ViewRoomInfo();
        }

        private void ViewRoomInfo()
        {
            UILobby.Instance.ViewRoomInfo(new EnterRoomParams { RoomName = _roomInfo.Name });
        }

        public void OnDoubleClick()
        {
            JoinRoom();
        }

        private void JoinRoom()
        {
            UILobby.Instance.JoinRoom(new EnterRoomParams { RoomName = _roomInfo.Name });
        }
    }
}