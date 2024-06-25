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
        [SerializeField] private AnimationClip _attack;
        [SerializeField] private AnimationClip _die;
        
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

        private AnimationClip GetClip(ZombieState state)
        {
            switch (state)
            {
                case ZombieState.Sleep:
                case ZombieState.Idle:
                    return _idle;
                    break;
                case ZombieState.Chase:
                    return _run;
                    break;
                case ZombieState.Attack:
                    return _attack;
                    break;
                case ZombieState.Die:
                    return _die;
                    break;
            }

            Debug.LogWarning($"{state} State AnimationClip not found.");
            return null;
        }

        private void AnimationState(ZombieAgent agent, ZombieState currentState)
        {
            if (agent.State == currentState)
                return;

            var prevState = agent.State;
            agent.State = currentState;
            var clip = GetClip(currentState);
            if (clip == null)
                return;
            GPUICrowdAPI.StartAnimation(agent.crowdPrefab, clip, -1, 1, 0.5f);
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