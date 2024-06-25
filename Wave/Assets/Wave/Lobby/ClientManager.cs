using UnityEngine;

namespace Wave.Lobby
{
    public class ClientManager : MonoBehaviour
    {
        public static QuantumLoadBalancingClient Client { get; private set; }

        private void Awake()
        {
            Client = new QuantumLoadBalancingClient(PhotonServerSettings.Instance.AppSettings.Protocol);
            
            DontDestroyOnLoad(gameObject);
        }
        
        private void OnDestroy()
        {
            if (Client is { IsConnected: true })
            {
                Client.Disconnect();
            }
        }
        
        private void Update()
        {
            Client?.Service();
        }
    }
}