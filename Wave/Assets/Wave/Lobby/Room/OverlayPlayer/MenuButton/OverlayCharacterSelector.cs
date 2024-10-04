using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Wave.Lobby.Room.OverlayPlayer
{
    public class OverlayCharacterSelector : Graphic, IPointerClickHandler
    {
        [SerializeField] private OverlayCharacter _overlayCharacter;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right) return;
            _overlayCharacter.OnClicked();
        }
    }
}