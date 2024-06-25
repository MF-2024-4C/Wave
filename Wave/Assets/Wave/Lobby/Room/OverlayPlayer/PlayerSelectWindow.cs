using System.Collections;
using System.Collections.Generic;
using Michsky.UI.Heat;
using UnityEngine;
using UnityEngine.Events;
using Wave.Lobby.Room;

public class PlayerSelectWindow : MonoBehaviour
{
    public static PlayerSelectWindow Instance { get; private set; }

    public Photon.Realtime.Player _player;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }
    }

    public void Show(Photon.Realtime.Player player)
    {
        _player = player;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Hide();
        }
    }

    public void SetMaster()
    {
        Debug.Log($" SetMaster {_player.NickName}   ");
        UIRoom.Instance.SetMaster(_player);
    }

    public void ViewPlayerInfo()
    {
        Debug.Log($"ViewPlayerInfo {_player.NickName}");
        UIRoom.Instance.ViewPlayerInfo(_player);
    }

    public void KickPlayer()
    {
        Debug.Log($"KickPlayer {_player.NickName}");
        UIRoom.Instance.KickPlayer(_player);
    }

    public void BanPlayer()
    {
           Debug.Log($"BanPlayer {_player.NickName}");
        UIRoom.Instance.BanPlayer(_player);
    }
}