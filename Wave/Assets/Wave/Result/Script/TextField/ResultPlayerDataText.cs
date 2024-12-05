using TMPro;
using UnityEngine;
using System.Reflection;
using DG.Tweening;

namespace Wave.Result
{
    /// <summary>
    /// プレイヤーデータに関するテキスト
    /// </summary>
    public class ResultPlayerDataText : ResultTextBase
    {
        [SerializeField] private TextMeshProUGUI _text;
        [HideInInspector] public string fieldName;
        
        public override void Show(ResultPlayerData playerData, ResultGameData gameData, float animTime)
        {
            if (string.IsNullOrEmpty(fieldName)) return;
            PropertyInfo propertyInfo = playerData.GetType().GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo != null && propertyInfo.CanRead)
            {
                object value = propertyInfo.GetValue(playerData);
                _text.text = value?.ToString() ?? "N/A";
                _tweener = _text.DOFade(1, animTime).OnComplete(() => _tweener = null);
            }
            else
            {
                Debug.LogWarning($"Property '{fieldName}' could not be found or is not readable.");
            }
        }
        
        public override void Hide(float animTime)
        {
            _text.DOFade(0, animTime);
        }

        public override void Hide()
        {
            Color color = _text.color;
            color.a = 0;
            _text.color = color;
        }

        public override void StopAnimation()
        {
            if (_tweener != null)
            {
                _tweener.Kill();
            }
        }
    }
}