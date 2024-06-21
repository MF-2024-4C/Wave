using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Michsky.UI.Heat;
using Photon.Realtime;
using Quantum;
using Quantum.Demo;
using TMPro;
using UnityEngine;
using Wave.Lobby.Room.OverlayPlayer;

namespace Wave.Lobby.Room
{
    public class UIRoom : MonoBehaviour, IInRoomCallbacks, IOnEventCallback
    {
        public static UIRoom Instance;

        [SerializeField] private ButtonManager _playButtonManager;
        [SerializeField] private TMP_InputField _roomNameInputField;
        [SerializeField] private SwitchManager _privateSwitchManager;

        [SerializeField] private OverlayCharacterManager _overlayCharacterManager;

        [SerializeField] private RuntimeConfigContainer _config;

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

        public void OnStartClicked()
        {
            if (!WaveUIConnect.Client.LocalPlayer.IsMasterClient) return;

            if (WaveUIConnect.Client.OpRaiseEvent(
                    (byte)WaveUIConnect.PhotonEventCode.StartGame,
                    null,
                    new RaiseEventOptions { Receivers = ReceiverGroup.All },
                    SendOptions.SendReliable))
            {
                Debug.Log("ゲームを開始します");
            }
            else
            {
                Debug.Log("ゲームを開始できませんでした");
            }
        }

        public void OnEvent(EventData photonEvent)
        {
            switch ((WaveUIConnect.PhotonEventCode)photonEvent.Code)
            {
                case WaveUIConnect.PhotonEventCode.StartGame:

                    Debug.Log("ゲーム開始イベントを受信しました");
                    WaveUIConnect.Client.CurrentRoom.CustomProperties.TryGetValue("MAP-GUID", out var mapGuid);

                    if (mapGuid == null)
                    {
                        Debug.Log("マップが選択されていません");
                        return;
                    }

                    StartGame((long)mapGuid);
                    break;
                default:
                    break;
            }
        }

        private void StartGame(AssetGuid mapGuid)
        {
            if (QuantumRunner.Default != null)
            {
                Debug.Log("ゲームが既に開始されています");
                return;
            }

            var config = _config.Config;

            config.Map.Id = mapGuid;

            var param = new QuantumRunner.StartParameters
            {
                RuntimeConfig = config,
                DeterministicConfig = DeterministicSessionConfigAsset.Instance.Config,
                ReplayProvider = null,
                GameMode = Photon.Deterministic.DeterministicGameMode.Multiplayer,
                FrameData = null,
                InitialFrame = 0,
                PlayerCount = WaveUIConnect.Client.CurrentRoom.MaxPlayers,
                LocalPlayerCount = 1,
                RecordingFlags = RecordingFlags.None,
                NetworkClient = WaveUIConnect.Client,
                StartGameTimeoutInSeconds = 10.0f
            };

            var clientId = WaveUIConnect.Client.LocalPlayer.NickName;

            _overlayCharacterManager.AllRemoveOverlayCharacter();

            QuantumRunner.StartGame(clientId, param);

            ReconnectInformation.Refresh(WaveUIConnect.Client, TimeSpan.FromMinutes(1));
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
            _roomNameInputField.text = WaveUIConnect.Client.CurrentRoom.CustomProperties["ROOM-NAME"].ToString();
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

            var customProperties = new Hashtable { { "ROOM-NAME", _roomNameInputField.text } };
            WaveUIConnect.Client.CurrentRoom.SetCustomProperties(customProperties);
        }

        public void OnPrivateSwitchChanged()
        {
            if (!WaveUIConnect.Client.LocalPlayer.IsMasterClient) return;
            Debug.Log($"部屋の公開設定を{!_privateSwitchManager.isOn}に変更します");
            WaveUIConnect.Client.CurrentRoom.IsVisible = !_privateSwitchManager.isOn;
        }

        public void LeaveRoom()
        {
            WaveUIConnect.Client.OpLeaveRoom(false);

            _overlayCharacterManager.AllRemoveOverlayCharacter();

            WaveUIConnect.Client.OpLeaveLobby();
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