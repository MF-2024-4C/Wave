using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Quantum;
using UnityEngine;
using UnityEngine.Animations;
using LayerMask = Quantum.LayerMask;

namespace Wave.Player
{
    public class PlayerSetup : MonoBehaviour
    {
        [SerializeField] private EntityView _entityView;
        [SerializeField] private EntityPrototype _entityPrototype;
        [SerializeField] private GameObject _virtualCameraPrefab;
        [SerializeField] private GameObject _playerModel;
        [SerializeField] private PlayerAnimation _playerAnimation;
        
        
        [Header("Layer Mask")] [SerializeField]
        private LayerMask _localLayer;

        [SerializeField] private LayerMask _otherLayer;

        public void OnEntityInstantiated()
        {
            QuantumGame game = QuantumRunner.Default.Game;
            Frame frame = game.Frames.Verified;
            
            ConstraintSource moveSource = new ConstraintSource();
            ConstraintSource rotSource = new ConstraintSource();
            
            if (frame.TryGet(_entityView.EntityRef, out PlayerLink playerLink))
            {
                frame.TryGet(_entityView.EntityRef,out PhysicsCollider3D collider3D);
                if (!game.PlayerIsLocal(playerLink.Player))
                {
                    //ローカルじゃない場合
                    _entityPrototype.PhysicsCollider.Layer = _otherLayer.BitMask;
                    InstantiateOtherPlayerModel();
                    return;
                }

                //ローカルの場合
                _entityPrototype.PhysicsCollider.Layer = _localLayer.BitMask;
                rotSource.sourceTransform = Camera.main.transform;
                rotSource.weight = 1;
                moveSource.sourceTransform = this.transform;
                moveSource.weight = 1;
                InstantiateLocalPlayerModel(moveSource, rotSource);
                
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
        
        public void InstantiateLocalPlayerModel(ConstraintSource moveSource, ConstraintSource rotSource)
        {
            var model = GameObject.Instantiate(_playerModel);

            if (model.TryGetComponent<PositionConstraint>(out PositionConstraint positionConstraint))
            {
                positionConstraint.constraintActive = true;
                positionConstraint.AddSource(moveSource);
            }

            if (model.TryGetComponent<RotationConstraint>(out RotationConstraint rotationConstraint))
            {
                rotationConstraint.constraintActive = true;
                rotationConstraint.AddSource(rotSource);
            }
            
            var meshRenderer = model.GetComponentInChildren<SkinnedMeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false;
            }
            
            if(model.TryGetComponent<Animator>(out Animator anim)) _playerAnimation.SetAnimator(anim);
        }
        
        public void InstantiateOtherPlayerModel()
        {
            var model = GameObject.Instantiate(_playerModel, this.transform);

            if (model.TryGetComponent<PositionConstraint>(out PositionConstraint positionConstraint))
                positionConstraint.enabled = false;
            if (model.TryGetComponent<RotationConstraint>(out RotationConstraint rotationConstraint))
                rotationConstraint.enabled = false;
            
            model.transform.localPosition = Vector3.zero;
            
            var meshRenderer = model.GetComponentInChildren<SkinnedMeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = true;
            }
            
            if(model.TryGetComponent<Animator>(out Animator anim)) _playerAnimation.SetAnimator(anim);
        }
    }
}