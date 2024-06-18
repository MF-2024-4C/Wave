using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class WaveUIConnect : MonoBehaviour, IConnectionCallbacks
{
    public static QuantumLoadBalancingClient Client { get; set; }
    [SerializeField] private string _appVersion = "Development";
    private string _fixedRegion = "jp";

    private void Awake()
    {
        var appSettings = PhotonServerSettings.CloneAppSettings(PhotonServerSettings.Instance.AppSettings);
        Client = new QuantumLoadBalancingClient(PhotonServerSettings.Instance.AppSettings.Protocol);
        appSettings.FixedRegion = _fixedRegion;
        appSettings.AppVersion = _appVersion;
        Client.AddCallbackTarget(this);
        if (Client.ConnectUsingSettings(appSettings, "Player test"))
        {
            Debug.Log("サーバーに接続中...");
        }
        else
        {
            Debug.Log("サーバーに接続できませんでした");
        }
        //Client.ConnectToMasterServer();
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

    #region IConnectionCallbacks

    public void OnConnected()
    {
    }

    public void OnConnectedToMaster()
    {
        Debug.Log("サーバーに接続した");
        Client.OpJoinLobby(null);
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