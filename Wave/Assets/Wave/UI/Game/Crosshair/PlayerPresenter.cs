using System;
using Quantum;
using UnityEngine;
using Wave.UI.Game;

namespace Wave
{
    public class PlayerPresenter : MonoBehaviour
    {
        private EntityView _entityView;
        private Crosshair _crosshair;
        private bool _isLocalPlayer;
        
        private bool _isInstantiated = false;

        private void Awake()
        {
            _entityView = GetComponentInChildren<EntityView>();
        }
        
        public void InstantiatePlayer(QuantumGame game)
        {
            var frame = game.Frames.Predicted;
            var playerLink = frame.Get<PlayerLink>(_entityView.EntityRef);
            _isLocalPlayer = game.PlayerIsLocal(playerLink.Player);
            _isInstantiated = true;
            PlayerSetup();
            LocalPlayerSetup();
            RemotePlayerSetup();
        }

        private void PlayerSetup()
        {
            
        }
        
        private void LocalPlayerSetup()
        {
            if (!_isLocalPlayer) return;
        }
        
        private void RemotePlayerSetup()
        {
            if (_isLocalPlayer) return;
        }
    }
}