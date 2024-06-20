using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

namespace Wave.Lobby
{
    public class WaveUIConnect : MonoBehaviour, IConnectionCallbacks
    {
        public static QuantumLoadBalancingClient Client { get; set; }
        [SerializeField] private string _appVersion = "Development";
        private string _fixedRegion = "jp";

        [SerializeField] private PlayerNameSetter _playerNameSetter;

        private void Awake()
        {
          
            Client = new QuantumLoadBalancingClient(PhotonServerSettings.Instance.AppSettings.Protocol);
    
            
            Client.AddCallbackTarget(this);
        }

        private void Start()
        {
            var appSettings = PhotonServerSettings.CloneAppSettings(PhotonServerSettings.Instance.AppSettings);
            appSettings.FixedRegion = _fixedRegion;
            appSettings.AppVersion = _appVersion;

            if (Client.ConnectUsingSettings(appSettings, "Player test"))
            {
                Debug.Log("サーバーに接続中...");
                LoadingScreen.Instance.ShowLoading("Connecting to server...");
            }
            else
            {
                Debug.Log("サーバーに接続できませんでした");
            }
        }

        private void OnDestroy()
        {
            Client?.RemoveCallbackTarget(this);
            if (Client is { IsConnected: true })
            {
                Client.Disconnect();
            }
        }

        private void Update()
        {
            Client?.Service();
        }

        public void JoinToLobby()
        {
            Debug.Log("ロビーに参加中...");
            LoadingScreen.Instance.ShowLoading("Joining Lobby...");
            Client.OpJoinLobby(null);
        }

        #region IConnectionCallbacks

        public void OnConnected()
        {
        }

        public void OnConnectedToMaster()
        {
            Debug.Log("サーバーに接続した");
            LoadingScreen.Instance.HideLoading();

            Client.LocalPlayer.NickName = PlayerProfile.PlayerProfile.Instance.PlayerName;
            _playerNameSetter.ViewPlayerName();
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