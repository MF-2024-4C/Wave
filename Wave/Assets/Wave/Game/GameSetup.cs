using UnityEngine;

namespace Wave.Game
{
    public class GameSetup : QuantumCallbacks
    {
        private void Awake()
        {
            CursorLockedAndHidden();
        }
        
        
        private static void CursorLockedAndHidden()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}