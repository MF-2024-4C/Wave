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

        private void Awake()
        {
            Debug.Log("イベント購買");
            _subscription = QuantumEvent.Subscribe<EventPlayerSpawnEvent>(this, SpawnPlayer);
        }

        private void Start()
        {
            var frame = QuantumRunner.Default.Game.Frames.Predicted;
            foreach (var (entity, component) in frame.GetComponentIterator<PlayerLink>())
            {
                var e = new EventPlayerSpawnEvent
                {
                    Game = QuantumRunner.Default.Game,
                    EntityRef = entity,
                    PlayerLink = component
                };
                SpawnPlayer(e);
            }
        }

        private void OnDisable()
        {
            QuantumEvent.Unsubscribe(_subscription);
        }

        private void SpawnPlayer(EventPlayerSpawnEvent e)
        {
            Debug.Log($"イベント実行:{e.EntityRef} {e.PlayerLink.Player}");
            var frame = e.Game.Frames.Predicted;
            for (int i = 0; i < PlayerStateUIList.Count; i++)
            {
                PlayerStateUI stateUI = PlayerStateUIList[i];
                if (stateUI.IsSetEntityView()) continue;
                string playerName = "Unknown";
                if (ClientManager.Client != null)
                {
                    var playerList = ClientManager.Client.CurrentRoom.Players;
                    var actorId = frame.PlayerToActorId(e.PlayerLink.Player);
                    playerName = actorId == null ? "Unknown" : playerList[(int)actorId].NickName;
                }

                stateUI.ActiveStateUI(e.EntityRef, e.PlayerLink, playerName);
                return;
            }
        }
    }
}