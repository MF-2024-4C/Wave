using UnityEngine;

namespace Wave.UI.Game.Status
{
    /// <summary>
    /// UI上に表示されるアイテムの情報を保持するクラス
    /// </summary>
    public class ItemViewInfo
    {
        private Sprite _itemSprite;
        public Sprite ItemSprite => _itemSprite;
        
        private string _itemName;
        public string ItemName => _itemName;
        
        private string _itemDescription;
        public string ItemDescription => _itemDescription;
        
        private int _itemCount;
        public int ItemCount => _itemCount;
        
        private int _itemStack;
        public int ItemStack => _itemStack;


        public ItemViewInfo(Sprite itemSprite, string itemName, string itemDescription, int itemCount,int itemStack)
        {
            _itemSprite = itemSprite;
            _itemName = itemName;
            _itemDescription = itemDescription;
            _itemCount = itemCount;
            _itemStack = itemStack;
        }
    }
}