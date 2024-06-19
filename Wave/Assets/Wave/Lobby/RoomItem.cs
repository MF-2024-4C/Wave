using ExitGames.Client.Photon;
using JetBrains.Annotations;
using Michsky.UI.Heat;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Wave.Lobby
{
    public class RoomItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _roomName;
        [SerializeField] private TextMeshProUGUI _playerCount;
        private RoomInfo _roomInfo;
        
        public void SetRoomInfo(RoomInfo roomInfo)
        {
            _roomName.text = roomInfo.Name;
            _playerCount.text = $"{roomInfo.PlayerCount}/{roomInfo.MaxPlayers}";

            _roomInfo = roomInfo;
        }

        public void OnClick()
        {
            
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