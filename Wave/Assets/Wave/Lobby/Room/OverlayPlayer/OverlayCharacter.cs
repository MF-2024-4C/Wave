using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Wave.Lobby.Room.OverlayPlayer
{
    public class OverlayCharacter : MonoBehaviour
    {
        public Photon.Realtime.Player Player;
        [SerializeField] private TextMeshProUGUI _playerNameText;

        public void SetPlayer(Photon.Realtime.Player player, string name)
        {
            Player = player;
            //_playerNameText.text = player.NickName;
            _playerNameText.text = name;
        }

        public void OnClicked()
        {
          
            if (Input.GetMouseButtonUp(1))
            {
                PlayerSelectWindow.Instance.Show(Player);
            }
        }
    }
}