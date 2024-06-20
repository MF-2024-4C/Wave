using System.Collections.Generic;
using ExitGames.Client.Photon;
using Michsky.UI.Heat;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Wave.Lobby
{
    public class UIRoom : MonoBehaviour, IInRoomCallbacks
    {
        public static UIRoom Instance;

        [SerializeField] private ButtonManager _playButtonManager;
        [SerializeField] private TMP_InputField _roomNameInputField;
        [SerializeField] private SwitchManager _privateSwitchManager;

        [SerializeField] private OverlayCharacterManager _overlayCharacterManager;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                Destroy(Instance.gameObject);
                Instance = this;
            }
        }

        private void Start()
        {
            WaveUIConnect.Client.AddCallbackTarget(this);
        }

        public void OnJoinRoom()
        {
            UpdateRoomControls();
            UpdateRoomInfo();
            UpdatePlayerInfo();
        }

        private void UpdateRoomControls()
        {
            bool isMasterClient = WaveUIConnect.Client.LocalPlayer.IsMasterClient;
            _playButtonManager.Interactable(isMasterClient);
            _roomNameInputField.interactable = isMasterClient;
            _privateSwitchManager.isInteractable = isMasterClient;
        }

        private void UpdateRoomInfo()
        {
            _roomNameInputField.text = WaveUIConnect.Client.CurrentRoom.Name;
            if (WaveUIConnect.Client.CurrentRoom.IsVisible)
                _privateSwitchManager.SetOff();
            else
                _privateSwitchManager.SetOn();
        }

        private void UpdatePlayerInfo()
        {
            _overlayCharacterManager.ViewOverlayCharacter(
                new List<Photon.Realtime.Player>(WaveUIConnect.Client.CurrentRoom.Players.Values));
        }

        public void OnRoomNameChanged()
        {
            if (!WaveUIConnect.Client.LocalPlayer.IsMasterClient) return;
            Debug.Log($"部屋名を{_roomNameInputField.text}に変更します");

            //TODO:部屋名の変更
        }

        public void OnPrivateSwitchChanged()
        {
            if (!WaveUIConnect.Client.LocalPlayer.IsMasterClient) return;
            Debug.Log($"部屋の公開設定を{!_privateSwitchManager.isOn}に変更します");
            WaveUIConnect.Client.CurrentRoom.IsVisible = !_privateSwitchManager.isOn;
        }

        public void LeaveRoom()
        {
            //TODO:部屋から退出
            WaveUIConnect.Client.OpLeaveRoom(false);

            _overlayCharacterManager.AllRemoveOverlayCharacter();
        }

        #region IInRoomCallbacks

        public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            Debug.Log("新しいプレイヤーが入室しました");
            _overlayCharacterManager.AddOverlayCharacter(newPlayer);
        }

        public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            Debug.Log("プレイヤーが退室しました");
            _overlayCharacterManager.RemoveOverlayCharacter(otherPlayer);
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (WaveUIConnect.Client.LocalPlayer.IsMasterClient) return;
            Debug.Log("部屋のプロパティが更新された");

            UpdateRoomInfo();
        }

        public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
        }

        public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
        {
            UpdateRoomControls();
        }

        #endregion
    }
}