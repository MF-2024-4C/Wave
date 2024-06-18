using System.Collections;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum;
using UnityEngine;
using UnityEngine.UI;

namespace Wave.Player
{
    public class PlayerStateUI : MonoBehaviour
    {
        [SerializeField] private Image _healthBar;
        
        private EntityView _playerEntityView;

        private void Update()
        {
            QuantumGame game = QuantumRunner.Default.Game;
            Frame frame = game.Frames.Verified;

            if (_playerEntityView == null) return;
            if(!frame.TryGet(_playerEntityView.EntityRef, out HealthComponent healthComponent)) return;
            var maxHealth = healthComponent.MaxHealth.AsFloat;
            var currentHealth = healthComponent.CurrentHealth.AsFloat;
            
            _healthBar.fillAmount = currentHealth / maxHealth;
        }
        
        public void SetPlayerEntityView(EntityView entityView) => _playerEntityView = entityView;
    }
}
