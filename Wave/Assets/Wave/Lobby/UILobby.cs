﻿using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Michsky.UI.Heat;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Wave.Lobby
{
    public class UILobby : MonoBehaviour, ILobbyCallbacks, IMatchmakingCallbacks
    {
        public static UILobby Instance;

        [SerializeField] private RoomViewer roomViewer;

        [SerializeField] private PanelManager _panelManager;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            WaveUIConnect.Client?.AddCallbackTarget(this);
        }

        #region ILobbyCallbacks

        public void OnJoinedLobby()
        {
            Debug.Log("ロビーに入室した");

            _panelManager.OpenPanelByIndex(0);
            _panelManager.ShowCurrentPanel();
        }

        public void JoinRoom(EnterRoomParams enterRoomParams)
        {
            WaveUIConnect.Client.OpJoinRoom(enterRoomParams);
        }

        public void RoomCreate()
        {
            long defaultMapGuid = 0;
            if (defaultMapGuid == 0)
            {
                // Fall back to the first map asset we find
                var allMapsInResources =
                    UnityEngine.Resources.LoadAll<MapAsset>(QuantumEditorSettings.Instance.DatabasePathInResources);
                defaultMapGuid = allMapsInResources[0].AssetObject.Guid.Value;
                Debug.Log($"defaultMapGuid: {defaultMapGuid}");
            }

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
                }
            };
            Random.InitState(DateTime.Now.Millisecond);
            enterRoomParams.RoomName = Random.Range(1, 1000000000).ToString();
            WaveUIConnect.Client.OpCreateRoom(enterRoomParams);
        }

        public void OnLeftLobby()
        {
        }

        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
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
        }

        public void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("部屋の作成に失敗した");
        }

        public void OnJoinedRoom()
        {
            Debug.Log("部屋に入室した");
            _panelManager.OpenPanelByIndex(1);
            UIRoom.Instance.OnJoinRoom();
        }

        public void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("部屋の入室に失敗した");
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {
        }

        public void OnLeftRoom()
        {
        }

        #endregion
    }
}