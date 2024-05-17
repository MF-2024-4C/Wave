using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Quantum;
using UnityEngine;
using LayerMask = Quantum.LayerMask;

namespace Wave.Player
{    
    public class PlayerSetup : MonoBehaviour
    {
        [SerializeField] private EntityView _entityView;
        [SerializeField] private GameObject _virtualCameraPrefab;
        [SerializeField] private GameObject _localPlayerModel;
        [SerializeField] private GameObject _ohterPlayerModel;
        [SerializeField] private PlayerAnimation _playerAnimation;

        public void OnEntityInstantiated()
        {
            QuantumGame game = QuantumRunner.Default.Game;
            Frame frame = game.Frames.Verified;
            Animator anim = null;
            if (frame.TryGet(_entityView.EntityRef, out PlayerLink playerLink))
            {
                if (!game.PlayerIsLocal(playerLink.Player))
                {
                    _localPlayerModel.SetActive(false);
                    if (_ohterPlayerModel.TryGetComponent<Animator>(out anim)) SetAnimator(anim);
                    return;
                }

                _ohterPlayerModel.SetActive(false);
                if (_localPlayerModel.TryGetComponent<Animator>(out anim)) SetAnimator(anim);
                GameObject virtualCameraObject = Instantiate(_virtualCameraPrefab);
                if (virtualCameraObject.TryGetComponent<CinemachineVirtualCamera>(
                        out CinemachineVirtualCamera virtualCamera))
                {
                    virtualCamera.m_Follow = this.transform;
                }
            }
        }

        private void SetAnimator(Animator anim)
        {
            _playerAnimation.SetAnimator(anim);
        }
    }
}
