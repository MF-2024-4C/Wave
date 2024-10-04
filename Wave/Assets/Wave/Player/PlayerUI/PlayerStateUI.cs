using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum;
using UnityEngine;
using UnityEngine.UI;
using Wave.Lobby;

namespace Wave.Player
{
    public class PlayerStateUI : MonoBehaviour
    {
        [SerializeField] private Image _healthBar;
        [SerializeField] private Text _nameText;

        private PlayerLink _playerLink;
        private EntityRef _playerEntityRef;

        private void Awake()
        {
            if (!IsSetEntityView())
            {
                this.gameObject.SetActive(false);
                return;
            }
        }

        public void ActiveStateUI(EntityRef entityRef, PlayerLink playerLink, string name)
        {
            _playerLink = playerLink;
            _playerEntityRef = entityRef;
            _nameText.text = name;
            
            this.gameObject.SetActive(true);
        }
        
        private void Update()
        {
            if (!IsSetEntityView())
            {
                this.gameObject.SetActive(false);
                return;
            }
            
            QuantumGame game = QuantumRunner.Default.Game;
            Frame frame = game.Frames.Verified;

            if(!frame.TryGet(_playerEntityRef, out HealthComponent healthComponent)) return;
            var maxHealth = healthComponent.MaxHealth.AsFloat;
            var currentHealth = healthComponent.CurrentHealth.AsFloat;
            
            _healthBar.fillAmount = currentHealth / maxHealth;
        }

        public bool IsSetEntityView()
        {
            return (_playerEntityRef != EntityRef.None);
        }
    }
}
