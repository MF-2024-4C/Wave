using UnityEngine;

namespace Wave.Game
{
    public class GameSetup : QuantumCallbacks
    {
        private void Awake()
        {
            CursorLockedAndHidden();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CursorVisible();
            }
            else if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                CursorLockedAndHidden();
            }
        }

        private static void CursorLockedAndHidden()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Debug.Log("Cursor locked and hidden");
        }

        private static void CursorVisible()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Debug.Log("Cursor visible");
        }
    }
}