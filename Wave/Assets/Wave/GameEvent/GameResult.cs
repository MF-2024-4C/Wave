using System;
using System.Collections;
using DG.Tweening;
using Quantum;
using UnityEngine;
using UnityEngine.UI;
using Wave.Lobby;

namespace Wave.GameEvent
{
    public class GameResult : MonoBehaviour
    {
        [SerializeField] private int _gameEndId;
        
        private Image _fadeImage;
        private void Start()
        {
            _fadeImage = GetComponentInChildren<Image>();
            QuantumEvent.Subscribe<EventMissionComplete>(this, OnResult);
        }

        private void OnResult(EventMissionComplete e)
        {
            var settings = UnityDB.FindAsset<DefenseMissionTriggerConfigAsset>(e.MissionConfig.Id).Settings;
            if (settings.EndEventID != _gameEndId) return;
            Debug.Log("Game End");

            StartCoroutine(ResultAction());
        }
        private IEnumerator ResultAction()
        {
            yield return new WaitForSeconds(3);
            
            yield return _fadeImage.DOFade(1, 1).WaitForCompletion();
            yield return new WaitForSeconds(1);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            QuantumRunner.Default.Shutdown();
            LobbyOnlyObjectManager.Instance.LobbyEnable();
        }
    }
}