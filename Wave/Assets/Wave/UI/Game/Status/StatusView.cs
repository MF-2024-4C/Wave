using System.Collections.Generic;
using Quantum;
using UnityEngine;

namespace Wave.UI.Game.Status
{
    public class StatusView : MonoBehaviour
    {
        private int _selectedItemIndex;
        private List<ItemHolderView> _itemHolderViews;
        private ItemDetailView _itemDetailView;

        private void Awake()
        {
            Setup();
        }

        private void Setup()
        {
            _itemHolderViews = new List<ItemHolderView>();
            var holderViews = GetComponentsInChildren<ItemHolderView>();
            foreach (var holderView in holderViews)
            {
                _itemHolderViews.Add(holderView);
            }

            _itemDetailView = GetComponentInChildren<ItemDetailView>();
        }

        public void OnHandChanged(int index, ItemViewInfo itemViewInfo)
        {
            if (index < 0 || index >= _itemHolderViews.Count)
            {
                return;
            }
            
            _selectedItemIndex = index;
            
            if (_selectedItemIndex == index)
            {
                UpdateItemDetail(itemViewInfo);
            }
        }

        public void OnItemChanged(int index, ItemViewInfo itemViewInfo)
        {
            if (index < 0 || index >= _itemHolderViews.Count)
            {
                return;
            }

            _itemHolderViews[index].SetItemViewInfo(itemViewInfo);
            
            if (_selectedItemIndex == index)
            {
                UpdateItemDetail(itemViewInfo);
            }
        }

        public void OnItemSelected(int index, ItemViewInfo itemViewInfo)
        {
            if (index < 0 || index >= _itemHolderViews.Count)
            {
                return;
            }

            _itemHolderViews[_selectedItemIndex].OnUnFocus();
            _selectedItemIndex = index;
            _itemHolderViews[_selectedItemIndex].OnFocus();
            UpdateItemDetail(itemViewInfo);
        }
        
        private void UpdateItemDetail(ItemViewInfo itemViewInfo)
        {
            _itemDetailView.SetItemViewInfo(itemViewInfo);
        }
    }
}