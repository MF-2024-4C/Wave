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
        [SerializeField] private EntityPrototype _entityPrototype;
        [SerializeField] private GameObject _virtualCameraPrefab;
        [SerializeField] private GameObject _localPlayerModel;
        [SerializeField] private GameObject _ohterPlayerModel;
        [SerializeField] private PlayerAnimation _playerAnimation;

        [Header("Layer Mask")] [SerializeField]
        private LayerMask _localLayer;

        [SerializeField] private LayerMask _otherLayer;

        public void OnEntityInstantiated()
        {
            QuantumGame game = QuantumRunner.Default.Game;
            Frame frame = game.Frames.Verified;
            Animator anim = null;
            if (frame.TryGet(_entityView.EntityRef, out PlayerLink playerLink))
            {
                frame.TryGet(_entityView.EntityRef,out PhysicsCollider3D collider3D);
                if (!game.PlayerIsLocal(playerLink.Player))
                {
                    //ローカルじゃない場合
                    _localPlayerModel.SetActive(false);
                    _entityPrototype.PhysicsCollider.Layer = _otherLayer.BitMask;
                    if (_ohterPlayerModel.TryGetComponent<Animator>(out anim)) SetAnimator(anim);
                    return;
                }

                //ローカルの場合
                _ohterPlayerModel.SetActive(false);
                _entityPrototype.PhysicsCollider.Layer = _localLayer.BitMask;
                if (_localPlayerModel.TryGetComponent<Animator>(out anim)) SetAnimator(anim);
                GameObject virtualCameraObject = Instantiate(_virtualCameraPrefab);
                if (virtualCameraObject.TryGetComponent<CinemachineVirtualCamera>(
                        out CinemachineVirtualCamera virtualCamera))
                {
                    virtualCamera.m_Follow = this.transform;
                }
                
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void SetAnimator(Animator anim)
        {
            _playerAnimation.SetAnimator(anim);
        }
    }
}