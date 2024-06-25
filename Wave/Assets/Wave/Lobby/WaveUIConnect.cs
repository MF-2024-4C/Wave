using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

namespace Wave.Lobby
{
    public class WaveUIConnect : MonoBehaviour, IConnectionCallbacks
    {
        [SerializeField] private string _appVersion = "Development";
        private const string FixedRegion = "jp";

        [SerializeField] private PlayerNameSetter _playerNameSetter;
        
        public enum PhotonEventCode : byte {
            StartGame = 110,
            KickPlayer = 111,
        }


        private void Awake()
        {
            ClientManager.Client.AddCallbackTarget(this);
        }

        private void Start()
        {
            var appSettings = PhotonServerSettings.CloneAppSettings(PhotonServerSettings.Instance.AppSettings);
            appSettings.FixedRegion = FixedRegion;
            appSettings.AppVersion = _appVersion;

            if (ClientManager.Client.ConnectUsingSettings(appSettings, "Player test"))
            {
                Debug.Log("サーバーに接続中...");
                LoadingScreen.LoadingScreen.Instance.ShowLoading("Connecting to server...");
            }
            else
            {
                Debug.Log("サーバーに接続できませんでした");
            }
        }

        private void OnDestroy()
        {
            ClientManager.Client?.RemoveCallbackTarget(this);
            if (ClientManager.Client is { IsConnected: true })
            {
                ClientManager.Client.Disconnect();
            }
        }

        private void Update()
        {
            ClientManager.Client?.Service();
        }

        private void JoinToLobby()
        {
            Debug.Log("ロビーに参加中...");
            LoadingScreen.LoadingScreen.Instance.ShowLoading("Joining Lobby...");
            ClientManager.Client.OpJoinLobby(null);
        }

        #region IConnectionCallbacks

        public void OnConnected()
        {
        }

        public void OnConnectedToMaster()
        {
            Debug.Log("サーバーに接続した");
            LoadingScreen.LoadingScreen.Instance.HideLoading();

            ClientManager.Client.LocalPlayer.NickName = PlayerProfile.PlayerProfile.Instance.PlayerName;
            _playerNameSetter.ViewPlayerName();
            
            JoinToLobby();
        }

        public void OnDisconnected(DisconnectCause cause)
        {
            
        }

        public void OnRegionListReceived(RegionHandler regionHandler)
        {
        }

        public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
        }

        public void OnCustomAuthenticationFailed(string debugMessage)
        {
        }

        #endregion
    }
}