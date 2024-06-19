using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Michsky.UI.Heat;
using Photon.Realtime;
using Quantum.Demo;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Wave.Lobby
{
    public class UIRoom : MonoBehaviour,ILobbyCallbacks
    {
        public static UIRoom Instance;

        [SerializeField] private ButtonManager _playButtonManager;
        [SerializeField] private TMP_InputField _roomNameInputField;
        [SerializeField] private SwitchManager _privateSwitchManager;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void OnJoinRoom()
        {
            _playButtonManager.gameObject.SetActive(WaveUIConnect.Client.LocalPlayer.IsMasterClient);
            
            _roomNameInputField.interactable = WaveUIConnect.Client.LocalPlayer.IsMasterClient;
            _roomNameInputField.text = WaveUIConnect.Client.CurrentRoom.Name;
            
            _privateSwitchManager.isOn = WaveUIConnect.Client.CurrentRoom.IsVisible;
            _privateSwitchManager.isInteractable = WaveUIConnect.Client.LocalPlayer.IsMasterClient;
        }

        public void OnRoomNameChanged()
        {
            //WaveUIConnect.Client.CurrentRoom.Name = _roomNameInputField.text;
        }
        
        public void OnPrivateSwitchChanged()
        {
            WaveUIConnect.Client.CurrentRoom.IsVisible = _privateSwitchManager.isOn;
        }
        
        public void LeaveRoom()
        {
            WaveUIConnect.Client.OpLeaveRoom(true);
        }
        
        
        #region ILobbyCallbacks

        public void OnJoinedLobby()
        {
        }

        public void OnLeftLobby()
        {
        }

        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            if (WaveUIConnect.Client.CurrentRoom == null) return;
            foreach (var roomInfo in roomList.Where(roomInfo => Equals(roomInfo, WaveUIConnect.Client.CurrentRoom)))
            {
                _roomNameInputField.text = roomInfo.Name;
                _privateSwitchManager.isOn = roomInfo.IsVisible;
                break;
            }
        }

        public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
        }
        
        #endregion
    }
}