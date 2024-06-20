using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Wave.Lobby;

public class OverlayCharacter : MonoBehaviour
{
    public Player Player;
    [SerializeField] private TextMeshProUGUI _playerNameText;
    public void SetPlayer(Player player,string name)
    {
        Player = player;
        //_playerNameText.text = player.NickName;
        _playerNameText.text = name;
    }
}