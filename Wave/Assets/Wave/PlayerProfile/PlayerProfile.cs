using UnityEngine;

namespace Wave.PlayerProfile
{
    public class PlayerProfile
    {
        public static PlayerProfile Instance { get; } = new PlayerProfile();

        private const string PlayerNameKey = "PlayerName";

        public string PlayerName
        {
            get
            {
                var playerName = PlayerPrefs.GetString(PlayerNameKey);
                if (string.IsNullOrEmpty(playerName))
                    playerName = "Player" + Random.Range(0, 10000).ToString("0000");
                return playerName;
            }
            set => PlayerPrefs.SetString(PlayerNameKey, value);
        }
    }
}