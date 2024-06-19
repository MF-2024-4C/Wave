using UnityEngine;
using TMPro;

namespace Wave.Lobby
{
    public class PlayerNameSetter : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;

        private void Start()
        {
            _inputField.text = PlayerProfile.PlayerProfile.Instance.PlayerName;
        }

        public void SetPlayerName()
        {
            if (string.IsNullOrEmpty(_inputField.text))
            {
                _inputField.text = PlayerProfile.PlayerProfile.Instance.PlayerName;
                return;
            }

            PlayerProfile.PlayerProfile.Instance.PlayerName = _inputField.text;
        }
    }
}