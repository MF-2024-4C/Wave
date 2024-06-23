using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Michsky.UI.Heat;
using Photon.Realtime;
using Quantum;
using Quantum.Demo;
using TMPro;
using UnityEngine;
using Wave.Lobby.MapManager;
using Wave.Lobby.Room.OverlayPlayer;
using UnityEngine.UI;

namespace Wave.Lobby.Room
{
    public class UIRoom : MonoBehaviour, IInRoomCallbacks, IOnEventCallback
    {
        public static UIRoom Instance;

        [SerializeField] private ButtonManager _playButtonManager;
        [SerializeField] private TMP_InputField _roomNameInputField;
        [SerializeField] private SwitchManager _privateSwitchManager;
        [SerializeField] private BoxButtonManager _mapSelectButtonManager;

        [SerializeField] private OverlayCharacterManager _overlayCharacterManager;

        [SerializeField] private RuntimeConfigContainer _config;

        [SerializeField] private MapManager.MapManager _mapManager;
        [SerializeField] private TextMeshProUGUI _mapNameText, _mapDescriptionText;
        [SerializeField] private Image _mapPreviewImage;
        [SerializeField] private ModalWindowManager _mapSelectModalWindow;

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
            ClientManager.Client.AddCallbackTarget(this);
        }

        public void SetMap(MapInfo map)
        {
            Debug.Log($"マップを{map.MapName}に設定します");

            var customProperties = new Hashtable { { "MAP-GUID", map.MapAsset.AssetObject.Guid.Value } };
            ClientManager.Client.CurrentRoom.SetCustomProperties(customProperties);

            _mapSelectModalWindow.CloseWindow();
        }

        public void OnStartClicked()
        {
            if (!ClientManager.Client.LocalPlayer.IsMasterClient) return;

            if (ClientManager.Client.OpRaiseEvent(
                    (byte)WaveUIConnect.PhotonEventCode.StartGame,
                    null,
                    new RaiseEventOptions { Receivers = ReceiverGroup.All },
                    SendOptions.SendReliable))
            {
                Camera.main.Hide();
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
                    ClientManager.Client.CurrentRoom.CustomProperties.TryGetValue("MAP-GUID", out var mapGuid);

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
                PlayerCount = ClientManager.Client.CurrentRoom.MaxPlayers,
                LocalPlayerCount = 1,
                RecordingFlags = RecordingFlags.None,
                NetworkClient = ClientManager.Client,
                StartGameTimeoutInSeconds = 10.0f
            };

            var clientId = ClientManager.Client.LocalPlayer.NickName;

            _overlayCharacterManager.AllRemoveOverlayCharacter();

            QuantumRunner.StartGame(clientId, param);

            ReconnectInformation.Refresh(ClientManager.Client, TimeSpan.FromMinutes(1));
        }

        public void OnJoinRoom()
        {
            UpdateRoomControls();
            UpdateRoomInfo();
            UpdatePlayerInfo();
            UpdateMapInfo();
        }

        private void UpdateRoomControls()
        {
            bool isMasterClient = ClientManager.Client.LocalPlayer.IsMasterClient;
            _playButtonManager.Interactable(isMasterClient);
            _roomNameInputField.interactable = isMasterClient;
            _privateSwitchManager.isInteractable = isMasterClient;
            _mapSelectButtonManager.isInteractable = isMasterClient;
        }

        private void UpdateRoomInfo()
        {
            if (ClientManager.Client.CurrentRoom.CustomProperties.TryGetValue("ROOM-NAME", out var roomName))
                _roomNameInputField.text = roomName.ToString();

            if (ClientManager.Client.CurrentRoom.IsVisible)
                _privateSwitchManager.SetOff();
            else
                _privateSwitchManager.SetOn();
        }

        private void UpdateMapInfo()
        {
            if (!ClientManager.Client.CurrentRoom.CustomProperties.TryGetValue("MAP-GUID", out var mapGuid)) return;
            var mapInfo = GetMapInfoFromGuid((long)mapGuid);
            if (mapInfo == null) return;
            _mapNameText.text = mapInfo.MapName;
            _mapDescriptionText.text = mapInfo.MapDescription;
            _mapPreviewImage.sprite = mapInfo.PreviewImage;
        }

        public MapInfo GetMapInfoFromGuid(long mapGuid)
        {
            return _mapManager.Maps.FirstOrDefault(map => map.MapAsset.AssetObject.Guid.Value == mapGuid);
        }

        private void UpdatePlayerInfo()
        {
            _overlayCharacterManager.ViewOverlayCharacter(
                new List<Photon.Realtime.Player>(ClientManager.Client.CurrentRoom.Players.Values));
        }

        public void OnRoomNameChanged()
        {
            if (!ClientManager.Client.LocalPlayer.IsMasterClient) return;
            Debug.Log($"部屋名を{_roomNameInputField.text}に変更します");

            var customProperties = new Hashtable { { "ROOM-NAME", _roomNameInputField.text } };
            ClientManager.Client.CurrentRoom.SetCustomProperties(customProperties);
        }

        public void OnPrivateSwitchChanged()
        {
            if (!ClientManager.Client.LocalPlayer.IsMasterClient) return;
            Debug.Log($"部屋の公開設定を{!_privateSwitchManager.isOn}に変更します");
            ClientManager.Client.CurrentRoom.IsVisible = !_privateSwitchManager.isOn;
        }

        public void LeaveRoom()
        {
            ClientManager.Client.OpLeaveRoom(false);
            LoadingScreen.LoadingScreen.Instance.ShowLoading("Leaving Room...");

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
            Debug.Log("部屋のプロパティが更新された");

            UpdateMapInfo();

            if (ClientManager.Client.LocalPlayer.IsMasterClient) return;


            UpdateRoomInfo();
        }

        public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
        }

        public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
        {
            Debug.Log($" マスタークライアントが{newMasterClient.NickName}に変更されました");
            UpdateRoomControls();
        }

        #endregion
    }
}