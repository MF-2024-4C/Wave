using System.Collections;
using System.Collections.Generic;
using Quantum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wave.NavigationMarker.NavigationMarker;

namespace Wave.GameEvent
{
    public class MissionMessageViewer : MonoBehaviour
    {
        [SerializeField] private NavigationMarkerViewer _navigationMarkerViewer;
        [SerializeField] private Animator _animator;

        [SerializeField] private TextMeshProUGUI _mainText, _subText;
        [SerializeField] private Image _icon;

        private const string ShowTrigger = "Show";
        private const string HideTrigger = "Hide";

        private const float ShowDuration = 5f;

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _showSound, _hideSound;

        public void ShowMessage(string mainText, string subText)
        {
            _mainText.text = mainText;
            _subText.text = subText;

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

        private void Start()
        {
            QuantumEvent.Subscribe<EventMissionStart>(this, OnMissionStart);
            QuantumEvent.Subscribe<EventMissionComplete>(this, OnMissionComplete);
        }

        private void OnMissionStart(EventMissionStart e)
        {
            var configAsset = UnityDB.FindAsset<MissionConfigAsset>(e.MissionConfig.Id);
            if (configAsset == null)
            {
                Debug.LogError($"MissionConfigAsset not found for mission id {e.MissionConfig.Id}");
                return;
            }

            ShowMessage(configAsset.MissionName, configAsset.MissionDescription);
        }

        private void OnMissionComplete(EventMissionComplete e)
        {
            ShowMessage("Mission Complete", "Well done!");
            StartCoroutine(OnMissionCompleteCoroutine());
        }

        private IEnumerator OnMissionCompleteCoroutine()
        {
            yield return new WaitForSeconds(ShowDuration + 4f);
            _navigationMarkerViewer.SetNextMarkerPosition();
        }
    }
}