using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private PlayerUISO _playerUISO;
    [SerializeField] private GameObject _canInteractUI;

    private void Update()
    {
        if(_playerUISO.canInteract)
            _canInteractUI.SetActive(true);
        else
            _canInteractUI.SetActive(false);
    }
}
