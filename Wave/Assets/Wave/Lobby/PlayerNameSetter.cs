using UnityEngine;
using TMPro;

namespace Wave.Lobby
{
    public class PlayerNameSetter : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;
        public void ViewPlayerName()
        {
            _inputField.text = ClientManager.Client.LocalPlayer.NickName;
        }

        public void SetPlayerName()
        {
            
            if (string.IsNullOrEmpty(_inputField.text))
            {
                _inputField.text = PlayerProfile.PlayerProfile.Instance.PlayerName;
                return;
            }

            PlayerProfile.PlayerProfile.Instance.PlayerName = _inputField.text;
            ClientManager.Client.LocalPlayer.NickName = _inputField.text;
        }
    }
}