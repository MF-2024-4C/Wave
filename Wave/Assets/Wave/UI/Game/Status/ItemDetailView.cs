using System.Text;
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
            _itemCount.text = ZeroPaddingAndColoring(itemViewInfo.ItemCount, 3, Color.gray);
            _itemStackCount.text = itemViewInfo.ItemStack.ToString("D3");
        }

        private static string ZeroPaddingAndColoring(int num, int len, Color color)
        {
            var sb = new StringBuilder();
            sb.Append($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>");
            var digit = (num == 0) ? 0 : ((int)Mathf.Log10(num) + 1);
            for (var i = 0; i < len - digit; i++)
            {
                sb.Append("0");
            }

            sb.Append("</color>");
            sb.Append(num);
            return sb.ToString();
        }
    }
}