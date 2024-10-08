using System;
using System.Collections;
using System.Collections.Generic;
using Quantum;
using UnityEngine;

namespace Wave.Player
{
    public class PlayerUpdater : MonoBehaviour
    {
        [SerializeField] private EntityView _entityView;
        [SerializeField] private float _defaultCameraHeight = 1.75f;
        [SerializeField] private float _downCameraHeight = 1.0f;
        [SerializeField] private PlayerCameraSO _playerCameraSO;
        private DispatcherSubscription _downEventSubscript;
        private DispatcherSubscription _reviveEventSubscript;
        
        private void OnEnable()
        {
            _downEventSubscript = QuantumEvent.Subscribe<EventPlayerDownEvent>(this, OnPlayerDown);
            _reviveEventSubscript = QuantumEvent.Subscribe<EventPlayerReviveEvent>(this, OnPlayerRevive);
        }

        private void OnDisable()
        {
            QuantumEvent.Unsubscribe(_downEventSubscript);
            QuantumEvent.Unsubscribe(_reviveEventSubscript);
        }
        
        private void OnPlayerDown(EventPlayerDownEvent e)
        {
            if (!QuantumRunner.Default.Game.PlayerIsLocal(e.PlayerLink.Player)) return;
            _playerCameraSO.OnChangeCameraHeight?.Invoke(_downCameraHeight);
        }
        
        private void OnPlayerRevive(EventPlayerReviveEvent e)
        {
            if (!QuantumRunner.Default.Game.PlayerIsLocal(e.PlayerLink.Player)) return;
            _playerCameraSO.OnChangeCameraHeight?.Invoke(_defaultCameraHeight);
        }
    }
}
