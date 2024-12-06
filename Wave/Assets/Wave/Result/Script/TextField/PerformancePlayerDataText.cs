using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using DG.Tweening;
using Michsky.UI.Heat;
using TMPro;

namespace Wave.Result
{
    public class PerformancePlayerDataText : PerformanceTextBase
    {
        [HideInInspector]public string fieldName;
        [SerializeField] private GameObject _parent;
        [SerializeField] private GameObject _playerCard;
        private List<PlayerPerformanceCard> _playerPerformanceCards = new List<PlayerPerformanceCard>();
        private Dictionary<TextMeshProUGUI, Tweener> _activeTweeners = new Dictionary<TextMeshProUGUI, Tweener>();
        
        public override void Setup(List<ResultPlayerData> playerDataList)
        {
            for (int i = 0; i < playerDataList.Count; i++)
            {
                PlayerPerformanceCard playerPerformanceCard = Instantiate(_playerCard, _parent.transform).GetComponent<PlayerPerformanceCard>();
                playerPerformanceCard.PlayerName.text = playerDataList[i].PlayerName;
                _playerPerformanceCards.Add(playerPerformanceCard);
            }
        }
        
        public override void Show(List<ResultPlayerData> playerDataList, ResultGameData gameData, float animTime)
        {
            PropertyInfo propertyInfo = null;
            int count = 0;
            foreach(var performanceCard in _playerPerformanceCards)
            {
                if (string.IsNullOrEmpty(fieldName)) continue;
                propertyInfo = playerDataList[count].GetType().GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
                
                switch (Type.GetTypeCode(propertyInfo.PropertyType))
                {
                    case TypeCode.Single:
                        CountUp(performanceCard.FieldBox, (float)propertyInfo.GetValue(playerDataList[count]), animTime);
                        break;
                    case TypeCode.Int32:
                        CountUp(performanceCard.FieldBox, (int)propertyInfo.GetValue(playerDataList[count]), animTime);
                        break;
                    default:
                        Debug.LogWarning($"Property '{fieldName}' could not be found or is not readable.");
                        break;
                }
                performanceCard.PlayerName.text = playerDataList[count].PlayerName;
                count++;
            }
        }
        
        public override void Hide(float animTime)
        {
        }

        public override void Hide()
        {
            foreach (var playerPerformanceCard in _playerPerformanceCards)
            {
                playerPerformanceCard.FieldBox.text = "0";
            }
        }

        public override void StopAnimation()
        {
            foreach (var tweener in _activeTweeners.Values)
            {
                tweener.Kill(); // すべて停止
            }
            _activeTweeners.Clear(); // Dictionaryをクリア
        }

        private void CountUp(TextMeshProUGUI textBox , float value, float time)
        {
            Tweener tweener = DOTween.To(() => 0, x => textBox.text = x.ToString("F2"), value, time)
                .OnComplete(() => _activeTweeners.Remove(textBox));
            _activeTweeners[textBox] = tweener;
        }
        
        private void CountUp(TextMeshProUGUI textBox , int value, float time)
        {
            Tweener tweener = DOTween.To(() => 0, x => textBox.text = x.ToString("F2"), value, time)
                .OnComplete(() => _activeTweeners.Remove(textBox));
            _activeTweeners[textBox] = tweener;
        }
    }
}