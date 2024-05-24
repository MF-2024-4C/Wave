using Quantum.QuantumDemo;
using Photon.Deterministic;
using Quantum.Physics3D;

namespace Quantum
{
    public unsafe partial struct PlayerSys
    {
        public static void Move(Frame f, EntityRef entityRef, CharacterController3D* controller, PlayerSys* playerSys, Input input)
        {
            PlayerConfig config = f.FindAsset<PlayerConfig>(playerSys->Config.Id);
            CharacterController3DConfig cconfig = f.FindAsset<CharacterController3DConfig>(controller->Config.Id);

            cconfig.Braking = config.BreakPower;
            var animState = PlayerConfig.PAnimIdle;
            
            /*
            Input input = default;
            if (f.Unsafe.TryGetPointer(entityRef, out PlayerLink* playerLink))
            {
                input = *f.GetPlayerInput(playerLink->Player);
            }
            */

            if (input.PlayerJump.WasPressed && controller->Grounded)
            {
                controller->Jump(f, false, config.JumpPower);
                animState |= PlayerConfig.PAnimJump;
            }
            
            
            FP speed = config.WalkSpeed;
            if (input.PlayerDash)
            {
                speed = config.RunSpeed;
                animState |= PlayerConfig.PAnimRun;
            }
            controller->MaxSpeed = speed;
            controller->Move(f, entityRef, input.PlayerDirection.XOY);

            if (input.PlayerDirection != FPVector2.Zero)
            {
                animState |= PlayerConfig.PAnimMove;
            }

            if (!controller->Grounded)
            {
                animState |= PlayerConfig.PAnimFall;
            }
            else if ((playerSys->PlayerAnimState & PlayerConfig.PAnimFall) == PlayerConfig.PAnimFall)
            {
                animState |= PlayerConfig.PAnimGrounded;
            }
            
            playerSys->PlayerAnimState = animState;
        }
        
        public static void Rot(Frame f, EntityRef entity, Transform3D* transform, CharacterController3D* controller, PlayerSys* playerSys,Input input)
        {            
            //カメラの向いている方向にプレイヤーも回転する
            FPQuaternion targetRotation = FPQuaternion.LookRotation(input.CameraForwardDirection);
            targetRotation.X = 0;
            targetRotation.Z = 0;
            //transform->Rotation = FPQuaternion.Slerp(transform->Rotation, targetRotation, f.DeltaTime * config.RotationSpeed);
            //transform->Rotation = targetRotation;
            playerSys->TargetRotation = targetRotation;
            
        }

        public static void Interact(Frame f, EntityRef entity, Transform3D* transform, PlayerSys* playerSys, Input input)
        {
            if(input.Interact.WasPressed)
            {
                Log.Info("Interact input was pressed");
                PlayerConfig config = f.FindAsset<PlayerConfig>(playerSys->Config.Id);
                var hit = f.Physics3D.Raycast(transform->Position + config.InteractRayOffset, transform->Forward, config.InteractRayDistance);
                if (hit != null)
                {
                    //TODO::ヒットしたものがインタラクトできるものかチェック
                    //hit.Entity.
                    Hit3D hhit = (Hit3D)hit;
                    if(f.Unsafe.TryGetPointer<Interacter>(hhit.Entity, out Interacter* interacter))
                    {

                    }

                }
            }
        }

        public static void Recoil(FPVector2 recoil)
        {
            
        }
    }
}