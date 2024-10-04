using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wave.NavigationMarker.NavigationMarker;

namespace Wave.NavigationMarker.InstructionMessage
{
    public class InstructionMessageViewer : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        [SerializeField] private TextMeshProUGUI _mainText, _subText;
        [SerializeField] private Image _icon;

        private const string ShowTrigger = "Show";
        private const string HideTrigger = "Hide";

        private const float ShowDuration = 5f;
    
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _showSound, _hideSound;

        public void ShowMessage(NavigationMarkerData instructionMessage)
        {
            _mainText.text = instructionMessage.MainText;
            _subText.text = instructionMessage.SubText;
            _icon.sprite = instructionMessage.Icon;
            _icon.color = instructionMessage.Color;

            _animator.Play(ShowTrigger, 0, 0);
        
            if (_showSound)
                _audioSource.PlayOneShot(_showSound);
        
            StopAllCoroutines();
            StartCoroutine(HideMessage());
        }

        private IEnumerator HideMessage()
        {
            yield return new WaitForSeconds(ShowDuration);
        
            if (_hideSound)
                _audioSource.PlayOneShot(_hideSound);

            _animator.Play(HideTrigger, 0, 0);
        }
    }
}