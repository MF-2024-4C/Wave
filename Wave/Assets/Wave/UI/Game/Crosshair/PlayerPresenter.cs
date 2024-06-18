using System;
using Quantum;
using UnityEngine;
using Wave.UI.Game.Status;

namespace Wave.UI.Game
{
    public class PlayerPresenter : MonoBehaviour
    {
        private EntityView _entityView;

        // References
        // LocalPlayer

        private Crosshair _crosshair;
        private StatusView _statusView;

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

            _statusView = FindObjectsByType<StatusView>(FindObjectsSortMode.None)[0];
            QuantumEvent.Subscribe<EventChangeActiveWeapon>(this, OnWeaponChanged);
            QuantumEvent.Subscribe<EventInventoryUpdate>(this, OnInventoryUpdated);
            QuantumEvent.Subscribe<EventReloadComplete>(this, OnReloadCompleteLocal);
        }

        private void RemotePlayerSetup()
        {
            if (_isLocalPlayer) return;
        }

        private void OnFireLocal(EventFire e)
        {
            if (!_game.PlayerIsLocal(e.Player)) return;

            _crosshair.OnFire();
            
            var frame = _game.Frames.Predicted;
            var weapon = frame.Get<Quantum.Weapon>(e.Weapon);
            var index = ItemTypeIndex(weapon);
            var info = CreateItemViewInfo(weapon);
            
            _statusView.OnItemChanged(index, info);
        }

        private void OnReloadCompleteLocal(EventReloadComplete e)
        {
            if (!_game.PlayerIsLocal(e.Player)) return;
            var frame = _game.Frames.Predicted;
            var weapon = frame.Get<Quantum.Weapon>(e.Weapon);
            var index = ItemTypeIndex(weapon);
            var info = CreateItemViewInfo(weapon);
            _statusView.OnItemChanged(index, info);
        }
        
        private void OnWeaponChanged(EventChangeActiveWeapon e)
        {
            if (!_game.PlayerIsLocal(e.Player)) return;

            var asset = UnityDB.FindAsset<WeaponDataAsset>(e.NewWeapon.Id);

            _statusView.OnWeaponChanged(e);
        }

        private void OnInventoryUpdated(EventInventoryUpdate e)
        {
            if (!_game.PlayerIsLocal(e.Player)) return;
            var frame = _game.Frames.Predicted;
            var weapon = frame.Get<Quantum.Weapon>(e.WeaponRef);
            var index = ItemTypeIndex(weapon);
            var info = CreateItemViewInfo(weapon);
            _statusView.OnItemChanged(index, info);
        }

        private static ItemViewInfo CreateItemViewInfo(in Quantum.Weapon weapon)
        {
            var weaponDataAsset = UnityDB.FindAsset<WeaponDataAsset>(weapon.data.Id);
            return new ItemViewInfo(null, weaponDataAsset.name, "", weapon.currentAmmo);
        }

        private static int ItemTypeIndex(in Quantum.Weapon weapon)
        {
            return weapon.type switch
            {
                WeaponType.Primary => 0,
                WeaponType.Secondary => 1,
                WeaponType.Tertiary => 2,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}