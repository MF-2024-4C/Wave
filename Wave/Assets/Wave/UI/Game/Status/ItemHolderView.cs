using UnityEngine;
using UnityEngine.UI;

namespace Wave.UI.Game.Status
{
    public class ItemHolderView : MonoBehaviour
    {
        [SerializeField] private Image _focusImage;
        [SerializeField] private Image _itemImage;
        [SerializeField] private Text _itemName;

        public void OnFocus()
        {
            _focusImage.enabled = true;
        }
        
        public void OnUnFocus()
        {
            _focusImage.enabled = false;
        }
        
        public void SetItemViewInfo(ItemViewInfo itemViewInfo)
        {
            _itemImage.sprite = itemViewInfo.ItemSprite;
            _itemName.text = itemViewInfo.ItemName;
        }
    }
}