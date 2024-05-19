using GPUInstancer;
using GPUInstancer.CrowdAnimations;
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
        }

        public void Initialized()
        {
            ZombieAnimationController.Instance().AddZombie(_entityView.EntityRef, _instancePrefab);
        }
    }
}