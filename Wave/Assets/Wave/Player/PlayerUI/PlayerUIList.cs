using System;
using System.Collections;
using System.Collections.Generic;
using Quantum;
using UnityEngine;
using Wave.Lobby;

namespace Wave.Player
{
    public class PlayerUIList : MonoBehaviour
    {
        [SerializeField] private List<PlayerStateUI> PlayerStateUIList = new List<PlayerStateUI>();
        private DispatcherSubscription _subscription;

        private void OnEnable()
        {
            _subscription = QuantumEvent.Subscribe<EventPlayerSpawnEvent>(this, SpawnPlayer);
        }

        private void OnDisable()
        {
            QuantumEvent.Unsubscribe(_subscription);
        }

        private void SpawnPlayer(EventPlayerSpawnEvent e)
        {
            var frame = QuantumRunner.Default.Game.Frames.Predicted;
            foreach (PlayerStateUI stateUI in PlayerStateUIList)
            {
                if (stateUI.IsSetEntityView()) continue;
                stateUI.ActiveStateUI(e.EntityRef);
                return;
            }
        }
    }
}
