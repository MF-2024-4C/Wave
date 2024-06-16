using TMPro;
using UnityEngine;

namespace Wave.UI.Game.Status
{
    public class ItemDetailView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _itemName;
        [SerializeField] private TextMeshProUGUI _itemCount;
        [SerializeField] private TextMeshProUGUI _itemStackCount;
        
        public void SetItemViewInfo(ItemViewInfo itemViewInfo)
        {
            _itemName.text = itemViewInfo.ItemName;
            _itemCount.text = itemViewInfo.ItemCount.ToString();
            _itemStackCount.text = itemViewInfo.ItemStack.ToString();
        }
    }
}