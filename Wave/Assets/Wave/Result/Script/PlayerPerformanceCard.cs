using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Wave.Result
{
    public class PlayerPerformanceCard : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerName;
        [SerializeField] private TextMeshProUGUI _fieldBox;

        [SerializeField] private List<Image> _fieldImages = new List<Image>();
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _highlightColor;

        public TextMeshProUGUI PlayerName => _playerName;
        public TextMeshProUGUI FieldBox => _fieldBox;

        public void SetLocalPlayerColor()
        {
            foreach (Image fieldImage in _fieldImages)
            {
                fieldImage.color = _highlightColor;
            }
        }

        public void SetOtherPlayerColor()
        {
            foreach (Image fieldImage in _fieldImages)
            {
                fieldImage.color = _defaultColor;
            }
        }
    }
}