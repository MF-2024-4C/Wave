using TMPro;
using UnityEngine;

namespace Wave.Result
{
    public class PlayerPerformanceCard : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerName;
        [SerializeField] private TextMeshProUGUI _fieldBox;
        
        public TextMeshProUGUI PlayerName => _playerName;
        public TextMeshProUGUI FieldBox => _fieldBox;
    }
}