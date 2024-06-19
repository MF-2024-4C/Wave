using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class OverlayCharacter : MonoBehaviour
{
    public Player Player;
    public void SetPlayer(Player player)
    {
        Player = player;
    }
}