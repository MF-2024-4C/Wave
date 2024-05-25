using System;
using Quantum;
using UnityEngine;
using LayerMask = UnityEngine.LayerMask;

namespace Wave.Demo
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private GameObject _target;
        [SerializeField] private EntityView _entityView;
        private void Awake()
        {
        }

        public void OnEntityInstantiated()
        {
            QuantumGame game = QuantumRunner.Default.Game;
            Frame frame = game.Frames.Verified;

            if (frame.TryGet(_entityView.EntityRef, out PlayerLink playerLink))
            {
                if (!game.PlayerIsLocal(playerLink.Player)) return;
                _target.layer = LayerMask.NameToLayer("LocalPlayer");
            }
        }
    }
}