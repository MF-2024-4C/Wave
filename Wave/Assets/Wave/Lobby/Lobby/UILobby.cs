using System.Collections.Generic;
using ExitGames.Client.Photon;
using Michsky.UI.Heat;
using Photon.Realtime;
using UnityEngine;
using Wave.Lobby.Room;

namespace Wave.Lobby.Lobby
{
    public class UILobby : MonoBehaviour, IMatchmakingCallbacks, ILobbyCallbacks
    {
        public static UILobby Instance;

        [SerializeField] private RoomViewer roomViewer;

        [SerializeField] private PanelManager _panelManager;

        [SerializeField] private MapManager.MapManager _mapManager;

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

        public void ViewRoomInfo(EnterRoomParams enterRoomParams)
        {
            //TODO:プレイヤーに見せるべき部屋情報を表示する
        }

        public void JoinRoom(EnterRoomParams enterRoomParams)
        {
            LoadingScreen.LoadingScreen.Instance.ShowLoading("Joining Room...");
            ClientManager.Client.OpJoinRoom(enterRoomParams);
        }

        public void RoomCreate()
        {
            // Fall back to the first map asset we find
            var allMapsInResources =
                Resources.LoadAll<MapAsset>(QuantumEditorSettings.Instance.DatabasePathInResources);
            var defaultMapGuid = _mapManager.Maps[0].MapAsset.AssetObject.Guid.Value;
            
            var enterRoomParams = new EnterRoomParams
            {
                RoomOptions = new RoomOptions
                {
                    IsVisible = true,
                    MaxPlayers = 4,
                    Plugins = new[] { "QuantumPlugin" },
                    CustomRoomPropertiesForLobby = new[] { "MAP-GUID","ROOM-NAME" },
                    CustomRoomProperties = new Hashtable
                    {
                        { "MAP-GUID", defaultMapGuid },
                        { "ROOM-NAME", PlayerProfile.PlayerProfile.Instance.PlayerName + "'s Room" },
                    },
                    PlayerTtl = PhotonServerSettings.Instance.PlayerTtlInSeconds * 1000,
                    EmptyRoomTtl = 1000
                },
                RoomName = Random.Range(1, 100000000).ToString(),
            };
            ClientManager.Client.OpCreateRoom(enterRoomParams);

            LoadingScreen.LoadingScreen.Instance.ShowLoading("Creating Room...");
        }


        #region ILobbyCallbacks

        public void OnJoinedLobby()
        {
            Debug.Log("ロビーに参加しました");
            LoadingScreen.LoadingScreen.Instance.HideLoading();
            roomViewer.ClearAllRoomItems();
        }

        public void OnLeftLobby()
        {
            Debug.Log("ロビーに再接続中...");
            LoadingScreen.LoadingScreen.Instance.ShowLoading("Rejoining Lobby...");
            ClientManager.Client.OpJoinLobby(null);
        }

        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("部屋リストが更新されました");
            Debug.Log($"roomList.Count: {roomList.Count}");
            roomViewer.OnRoomListUpdate(roomList);
        }

        public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
        }

        #endregion

        #region IMatchmakingCallbacks

        public void OnFriendListUpdate(List<FriendInfo> friendList)
        {
        }

        public void OnCreatedRoom()
        {
            Debug.Log("部屋を作成した");
            LoadingScreen.LoadingScreen.Instance.HideLoading();
            LoadingScreen.LoadingScreen.Instance.ShowLoading("Joining Room...");
        }

        public void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("部屋の作成に失敗した");
            LoadingScreen.LoadingScreen.Instance.HideLoading();
        }

        public void OnJoinedRoom()
        {
            Debug.Log("部屋に入室した");
            _panelManager.OpenPanelByIndex(2);
            UIRoom.Instance.OnJoinRoom();
            LoadingScreen.LoadingScreen.Instance.HideLoading();
        }

        public void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("部屋の入室に失敗した");
            LoadingScreen.LoadingScreen.Instance.HideLoading();
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {
        }

        public void OnLeftRoom()
        {
            Debug.Log("部屋から退室した");
            LoadingScreen.LoadingScreen.Instance.HideLoading();
        }

        #endregion
    }
}