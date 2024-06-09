using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using GPUInstancer;
using GPUInstancer.CrowdAnimations;
using Quantum;
using UnityEngine;

namespace Wave.Exp.Enemy
{
    public class ZombieAnimationController : QuantumCallbacks
    {
        private class ZombieAgent
        {
            public GPUICrowdPrefab crowdPrefab;
            public ZombieState State;
            public EntityRef EntityRef;
        }

        private GPUICrowdManager _crowdManager;
        private List<ZombieAgent> _zombies;
        [SerializeField] private GPUICrowdPrototype _zombiePrototype;

        [Header("Animations")] [SerializeField]
        private AnimationClip _idle;

        [SerializeField] private AnimationClip _run;

        private static ZombieAnimationController _instance;

        public static ZombieAnimationController Instance()
        {
            return _instance;
        }

        private void Start()
        {
            _crowdManager = FindFirstObjectByType<GPUICrowdManager>();
            _zombies = new List<ZombieAgent>();
        }

        public void AddZombie(EntityRef entityRef, GPUICrowdPrefab prefab)
        {
            _zombies.Add(new ZombieAgent
            {
                crowdPrefab = prefab,
                EntityRef = entityRef,
            });
        }
        
        public void RemoveZombie(EntityRef entityRef)
        {
            // 末尾とスワップすることでO(1)で削除できる。
            // ただし、順序は保証されないので注意。
            for (var i = 0; i < _zombies.Count; i++)
            {
                if (_zombies[i].EntityRef != entityRef) continue;
                _zombies[i] = _zombies[^1];
                _zombies.RemoveAt(_zombies.Count - 1);
                return;
            }
        }
        

        public override void OnUpdateView(QuantumGame game)
        {
            var frame = game.Frames.Predicted;
            foreach (var zombie in _zombies)
            {
                var zombieComponent = frame.Get<Zombie>(zombie.EntityRef);
                var zombieState = zombieComponent.State;
                AnimationState(zombie, zombieState);
            }
        }

        private void AnimationState(ZombieAgent agent, ZombieState currentState)
        {
            if (agent.State == currentState)
                return;

            agent.State = currentState;

            switch (currentState)
            {
                case ZombieState.Sleep:
                case ZombieState.Idle:
                    GPUICrowdAPI.StartAnimation(agent.crowdPrefab, _idle, -1, 1, 0.5f);
                    break;
                case ZombieState.Chase:
                    GPUICrowdAPI.StartAnimation(agent.crowdPrefab, _run, -1, 1, 0.5f);
                    break;
                case ZombieState.Attack:
                    break;
                case ZombieState.Die:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Awake()
        {
            _instance = this;
        }

        private void OnDestroy()
        {
            _instance = null;
        }
    }
}