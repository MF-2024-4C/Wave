using System;
using GPUInstancer;
using GPUInstancer.CrowdAnimations;
using Quantum;
using UnityEngine;

namespace Wave.Exp.Enemy
{
    public class ZombieEntityView : MonoBehaviour
    {
        private EntityView _entityView;
        private GPUICrowdPrefab _instancePrefab;

        private void Awake()
        {
            _entityView = GetComponent<EntityView>();
            _instancePrefab = GetComponent<GPUICrowdPrefab>();

            _entityView.OnEntityInstantiated.AddListener(game => Initialized());
            _entityView.OnEntityDestroyed.AddListener(game => EntityDestroyed());
        }

        private void Initialized()
        {
            ZombieAnimationController.Instance().AddZombie(_entityView.EntityRef, _instancePrefab);
        }

        private void EntityDestroyed()
        {
            ZombieAnimationController.Instance().RemoveZombie(_entityView.EntityRef);
        }
    }
}