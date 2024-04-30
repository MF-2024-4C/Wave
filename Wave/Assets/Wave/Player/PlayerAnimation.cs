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

        public void Update()
        {
            QuantumGame game = QuantumRunner.Default.Game;
            Frame frame = game.Frames.Verified;

            if (!frame.TryGet(_entityView.EntityRef, out PlayerAnimInfo playerAnimInfo)) return;
            
            var state = playerAnimInfo.PlayerAnimState;
            if ((state & PlayerConfig.PAnimMove) == PlayerConfig.PAnimMove)
            {
                if((state & PlayerConfig.PAnimRun) == PlayerConfig.PAnimRun)
                {
                    Debug.Log("Run");
                }
                else
                {
                    Debug.Log("Walk");
                }
            }
            else
            {
                Debug.Log("Idle");
            }
                
            if ((state & PlayerConfig.PAnimJump) == PlayerConfig.PAnimJump)
            {
                Debug.Log("Jump");
            }

            if ((state & PlayerConfig.PAnimFall) == PlayerConfig.PAnimFall)
            {
                Debug.Log("Fall");
            }
            else if((state & PlayerConfig.PAnimGrounded) == PlayerConfig.PAnimGrounded)
            {
                Debug.Log("Grounded");
            }
        }
    }
}
