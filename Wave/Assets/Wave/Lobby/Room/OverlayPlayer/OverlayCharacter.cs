using TMPro;
using UnityEngine;

namespace Wave.Lobby.Room.OverlayPlayer
{
    public class OverlayCharacter : MonoBehaviour
    {
        public Photon.Realtime.Player Player;
        [SerializeField] private TextMeshProUGUI _playerNameText;
        public void SetPlayer(Photon.Realtime.Player player,string name)
        {
            Player = player;
            //_playerNameText.text = player.NickName;
            _playerNameText.text = name;
        }
    }
}