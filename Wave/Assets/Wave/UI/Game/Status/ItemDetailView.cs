using UnityEngine;
using UnityEngine.UI;

namespace Wave.UI.Game.Status
{
    public class ItemDetailView : MonoBehaviour
    {
        [SerializeField] private Image _itemImage;
        [SerializeField] private Text _itemName;
        [SerializeField] private Text _itemCount;
        [SerializeField] private Text _itemStackCount;
        
        public void SetItemViewInfo(ItemViewInfo itemViewInfo)
        {
            _itemImage.sprite = itemViewInfo.ItemSprite;
            _itemName.text = itemViewInfo.ItemName;
            _itemCount.text = itemViewInfo.ItemCount.ToString();
            _itemStackCount.text = itemViewInfo.ItemStack.ToString();
        }
    }
}