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
        
        private QuantumGame _game;

        private void Awake()
        {
            _entityView = GetComponentInChildren<EntityView>();
            _entityView.OnEntityInstantiated.AddListener(InstantiatePlayer);
        }
        
        public void InstantiatePlayer(QuantumGame game)
        {
            _game = game;
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

            _crosshair = FindObjectsByType<Crosshair>(FindObjectsSortMode.None)[0];
            QuantumEvent.Subscribe<EventFire>(this, OnFireLocal);
        }
        
        private void RemotePlayerSetup()
        {
            if (_isLocalPlayer) return;
        }
        
        private void OnFireLocal(EventFire e)
        {
            if (!_game.PlayerIsLocal(e.Player)) return;
            
            _crosshair.OnFire();
            
        }
    }
}