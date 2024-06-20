using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Michsky.UI.Heat;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Wave.Lobby
{
    public class UILobby : MonoBehaviour, IMatchmakingCallbacks, ILobbyCallbacks
    {
        public static UILobby Instance;

        [SerializeField] private RoomViewer roomViewer;

        [SerializeField] private PanelManager _panelManager;

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

        public void JoinRoom(EnterRoomParams enterRoomParams)
        {
            LoadingScreen.Instance.ShowLoading("Joining Room...");
            WaveUIConnect.Client.OpJoinRoom(enterRoomParams);
        }

        public void RoomCreate()
        {
            // Fall back to the first map asset we find
            var allMapsInResources =
                Resources.LoadAll<MapAsset>(QuantumEditorSettings.Instance.DatabasePathInResources);
            var defaultMapGuid = allMapsInResources[0].AssetObject.Guid.Value;
            Debug.Log($"defaultMapGuid: {defaultMapGuid}");

            var enterRoomParams = new EnterRoomParams
            {
                RoomOptions = new RoomOptions
                {
                    IsVisible = true,
                    MaxPlayers = 4,
                    Plugins = new[] { "QuantumPlugin" },
                    CustomRoomProperties = new Hashtable
                    {
                        { "HIDE-ROOM", false },
                        { "MAP-GUID", defaultMapGuid },
                    },
                    PlayerTtl = PhotonServerSettings.Instance.PlayerTtlInSeconds * 1000,
                    EmptyRoomTtl = PhotonServerSettings.Instance.EmptyRoomTtlInSeconds * 1000
                },
                RoomName = PlayerProfile.PlayerProfile.Instance.PlayerName + "'s Room",
            };
            WaveUIConnect.Client.OpCreateRoom(enterRoomParams);

            LoadingScreen.Instance.ShowLoading("Creating Room...");
        }


        #region ILobbyCallbacks

        public void OnJoinedLobby()
        {
            Debug.Log("ロビーに参加しました");
            LoadingScreen.Instance.HideLoading();
        }

        public void OnLeftLobby()
        {
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
            LoadingScreen.Instance.HideLoading();
            LoadingScreen.Instance.ShowLoading("Joining Room...");
        }

        public void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("部屋の作成に失敗した");
            LoadingScreen.Instance.HideLoading();
        }

        public void OnJoinedRoom()
        {
            Debug.Log("部屋に入室した");
            _panelManager.OpenPanelByIndex(2);
            UIRoom.Instance.OnJoinRoom();
            LoadingScreen.Instance.HideLoading();
        }

        public void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("部屋の入室に失敗した");
            LoadingScreen.Instance.HideLoading();
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {
        }

        public void OnLeftRoom()
        {
            Debug.Log("部屋から退室した");
        }

        #endregion
    }
}