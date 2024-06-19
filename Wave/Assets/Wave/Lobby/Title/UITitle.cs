using Michsky.UI.Heat;
using UnityEngine;

namespace Wave.Lobby
{
    public class UITitle : MonoBehaviour
    {
    
        [SerializeField] private PanelManager _panelManager;
    
    
        // Start is called before the first frame update
        void Start()
        {
            _panelManager.OpenPanelByIndex(0);
            _panelManager.ShowCurrentPanel();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
