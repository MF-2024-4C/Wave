using System;
using System.Collections;
using System.Collections.Generic;
using Quantum;
using UnityEngine;

namespace Wave.Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private EntityView _entityView;
        [SerializeField] private Animator _anim;

        private string _currentTriggerAnim;
        //private Animator _anim;
        private EntityComponentPlayerSys _playerSys;

        private void Start()
        {
            _currentTriggerAnim = "idle";
            _playerSys = GetComponent<EntityComponentPlayerSys>();
        }

        public void Update()
        {
            QuantumGame game = QuantumRunner.Default.Game;
            Frame frame = game.Frames.Verified;

            if (!frame.TryGet(_entityView.EntityRef, out PlayerSys playerLocalInfo)) return;
            
            UpdateAnimation(playerLocalInfo);
            UpdateRotation(playerLocalInfo);
        }

        private void UpdateAnimation(PlayerSys playerLocalInfo)
        {
            if (_anim == null) return;

            var state = playerLocalInfo.PlayerAnimState;
                        string currentTriggerAnim = "";
                        if ((state & PlayerConfig.PAnimMove) == PlayerConfig.PAnimMove)
                        {
                            if((state & PlayerConfig.PAnimRun) == PlayerConfig.PAnimRun)
                            {
                                //Debug.Log("Run");
                                currentTriggerAnim = "run";
                            }
                            else
                            {
                                //Debug.Log("Walk");
                                currentTriggerAnim = "walk";
                            }
                        }
                        else
                        {
                            //Debug.Log("Idle");
                            currentTriggerAnim = "idle";
                        }
            
                        bool isAir = false;
                        if ((state & PlayerConfig.PAnimJump) == PlayerConfig.PAnimJump)
                        {
                            //Debug.Log("Jump");
                            _anim.SetTrigger("jump");
                            isAir = true;
                        }
            
                        if ((state & PlayerConfig.PAnimFall) == PlayerConfig.PAnimFall)
                        {
                            //Debug.Log("Fall");
                            currentTriggerAnim = "fall";
                            isAir = true;
                        }
                        else if((state & PlayerConfig.PAnimGrounded) == PlayerConfig.PAnimGrounded)
                        {
                            //Debug.Log("Grounded");
                            currentTriggerAnim = "grounded";
                        }
                        
                        if(_currentTriggerAnim != currentTriggerAnim)
                        {
                            AllFalseTrigger();
                            _anim.SetTrigger(currentTriggerAnim);
                            _currentTriggerAnim = currentTriggerAnim;
                        }
                        _anim.SetBool("inAir", isAir);
        }

        private void UpdateRotation(PlayerSys playerLocalInfo)
        {
            var targetRotation = playerLocalInfo.TargetRotation;
            this.transform.rotation = Quaternion.Slerp(transform.rotation,
                playerLocalInfo.TargetRotation.ToUnityQuaternion(), 10f);
        }

        private void AllFalseTrigger()
        {
            _anim.ResetTrigger("idle");
            _anim.ResetTrigger("walk");
            _anim.ResetTrigger("run");
        }

        public void SetAnimator(Animator anim) { this._anim = anim; }
        
        
    }
}
