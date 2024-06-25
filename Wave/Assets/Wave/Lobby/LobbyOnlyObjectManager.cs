using UnityEngine;

namespace Wave.Lobby
{
    public class LobbyOnlyObjectManager : MonoBehaviour
    {
        public static LobbyOnlyObjectManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                Destroy(Instance.gameObject);
                Instance = this;
            }
        }

        public void LobbyEnable()
        {
            gameObject.SetActive(true);
        }

        public void LobbyDisable()
        {
            gameObject.SetActive(false);
        }
    }
}