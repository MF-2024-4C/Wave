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
using Random = UnityEngine.Random;

namespace Wave.Lobby.Room
{
    public class UIRoom : MonoBehaviour, IInRoomCallbacks, IOnEventCallback
    {
        public static UIRoom Instance;

        [SerializeField] private PanelManager _panelManager;

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

            var mapIndex = _mapManager.Maps.ToList().IndexOf(map);
            var customProperties = new Hashtable { { "MAP-INDEX", mapIndex } };
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
                    ClientManager.Client.CurrentRoom.CustomProperties.TryGetValue("MAP-INDEX", out var mapIndex);
                    if (mapIndex == null) return;
                    var mapGuid = _mapManager.Maps[(int)mapIndex].MapAsset.AssetObject.Guid.Value;
                    StartGame(mapGuid);
                    break;

                case WaveUIConnect.PhotonEventCode.KickPlayer:
                    LeaveRoom();
                    _panelManager.OpenPanelByIndex(1);
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

            //var clientId = ClientManager.Client.LocalPlayer.NickName;
            var clientId = Random.Range(1, 100000000).ToString();

            _overlayCharacterManager.AllRemoveOverlayCharacter();

            QuantumRunner.StartGame(clientId, param);

            ReconnectInformation.Refresh(ClientManager.Client, TimeSpan.FromMinutes(1));
            
            LobbyOnlyObjectManager.Instance.LobbyDisable();
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
            if (!ClientManager.Client.CurrentRoom.CustomProperties.TryGetValue("MAP-INDEX", out var mapIndex)) return;
            var mapInfo = GetMapInfoFromMapIndex((int)mapIndex);
            if (mapInfo == null) return;
            _mapNameText.text = mapInfo.MapName;
            _mapDescriptionText.text = mapInfo.MapDescription;
            _mapPreviewImage.sprite = mapInfo.PreviewImage;
        }

        public MapInfo GetMapInfoFromMapIndex(int mapIndex)
        {
            return _mapManager.Maps[mapIndex];
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

        #region PlayerSelectMenu

        public void SetMaster(Photon.Realtime.Player player)
        {
            //マスターをplayerに設定
            if (ClientManager.Client.LocalPlayer.IsMasterClient)
            {
                ClientManager.Client.CurrentRoom.SetMasterClient(player);
            }
        }

        public void ViewPlayerInfo(Photon.Realtime.Player player)
        {
        }

        public void KickPlayer(Photon.Realtime.Player player)
        {
            if (ClientManager.Client.OpRaiseEvent(
                    (byte)WaveUIConnect.PhotonEventCode.KickPlayer,
                    null,
                    new RaiseEventOptions { TargetActors = new[] { player.ActorNumber } },
                    SendOptions.SendReliable))
            {
                Debug.Log("プレイヤーをキックします");
            }
            else
            {
                Debug.Log("プレイヤーをキックできませんでした");
            }
        }

        public void BanPlayer(Photon.Realtime.Player player)
        {
        }

        #endregion
    }
}