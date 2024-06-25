using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wave.Lobby.Room;

namespace Wave.Lobby.Lobby
{
    public class RoomItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _roomName;
        [SerializeField] private TextMeshProUGUI _playerCount;
        [SerializeField] private TextMeshProUGUI _mapName;
        [SerializeField] private Image _mapPreview;
        private RoomInfo _roomInfo;

        public void SetRoomInfo(RoomInfo roomInfo)
        {
            _roomName.text = roomInfo.CustomProperties["ROOM-NAME"].ToString();
            _playerCount.text = $"{roomInfo.PlayerCount}/{roomInfo.MaxPlayers}";
            if (roomInfo.CustomProperties.TryGetValue("MAP-INDEX", out var mapIndex))
            {
                var mapInfo = UIRoom.Instance.GetMapInfoFromMapIndex((int)mapIndex);
                if (mapInfo != null)
                {
                    _mapName.text = mapInfo.MapName;
                    _mapPreview.sprite = mapInfo.PreviewImage;
                }
            }

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