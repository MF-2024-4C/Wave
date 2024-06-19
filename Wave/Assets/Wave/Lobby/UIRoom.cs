using Michsky.UI.Heat;
using UnityEngine;

namespace Wave.Lobby
{
    public class UIRoom : MonoBehaviour
    {
        
        public static UIRoom Instance;
        
        [SerializeField] private ButtonManager _playButtonManager;
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void OnJoinRoom()
        {
            //TODO: ホストかどうかの判断
            //_playButtonManager.gameObject.SetActive(true);
        }

        public void LeaveRoom()
        {
            WaveUIConnect.Client.OpLeaveRoom(true);
        }
    }
}