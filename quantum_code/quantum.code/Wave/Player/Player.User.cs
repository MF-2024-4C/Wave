using Quantum.QuantumDemo;
using Photon.Deterministic;
using Quantum.Physics3D;

namespace Quantum
{
    public unsafe partial struct PlayerSys
    {
        public static void Move(Frame f, EntityRef entityRef, CharacterController3D* controller, PlayerSys* playerSys,
            Input input)
        {
            CharacterController3DConfig cconfig = f.FindAsset<CharacterController3DConfig>(controller->Config.Id);

            cconfig.Braking = playerSys->BreakPower;
            var animState = PlayerConfig.PAnimIdle;

            if (input.PlayerJump.WasPressed && controller->Grounded)
            {
                controller->Jump(f, false, playerSys->JumpPower);
                animState |= PlayerConfig.PAnimJump;
            }


            FP speed = playerSys->WalkSpeed;
            if (input.PlayerDash)
            {
                speed = playerSys->RunSpeed;
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

        public static void Rot(Frame f, EntityRef entity, Transform3D* transform, CharacterController3D* controller,
            PlayerSys* playerSys, Input input)
        {
            //カメラの向いている方向にプレイヤーも回転する
            FPQuaternion targetRotation = FPQuaternion.LookRotation(input.CameraForwardDirection);
            targetRotation.X = 0;
            targetRotation.Z = 0;
            //transform->Rotation = FPQuaternion.Slerp(transform->Rotation, targetRotation, f.DeltaTime * config.RotationSpeed);
            //transform->Rotation = targetRotation;
            playerSys->TargetRotation = targetRotation;

            //インタラクト用にカメラの向いている方向を保存
            PlayerConfig config = f.FindAsset<PlayerConfig>(playerSys->Config.Id);
            config.CameraForwardDirection = input.CameraForwardDirection;
        }

        public static void Interact(Frame f, EntityRef entity, Transform3D* transform, PlayerSys* playerSys,
            Input input)
        {
            if (input.Interact.WasPressed)
            {
                Log.Info("Interact input was pressed");
                PlayerConfig config = f.FindAsset<PlayerConfig>(playerSys->Config.Id);
                var start = transform->Position + config.InteractRayOffset;
                var end = start + input.CameraForwardDirection * config.InteractRayDistance;
                var hits = f.Physics3D.LinecastAll(start, end);
                for (int i = 0; i < hits.Count; i++)
                {
                    var hit = hits[i];
                    if (f.Unsafe.TryGetPointer(hit.Entity, out Interacter* interacter))
                    {
                        if (!interacter->IsInteract) continue;
                        Interacter.Interact(f, entity, hit.Entity, interacter);
                        return;
                    }
                }
            }
        }

        public static void Recoil(FPVector2 recoil)
        {
        }

        public void SetConfig(Frame　f)
        {
            PlayerConfig config = f.FindAsset<PlayerConfig>(this.Config.Id);
            WalkSpeed = config.WalkSpeed;
            RunSpeed = config.RunSpeed;
            JumpPower = config.JumpPower;
            BreakPower = config.BreakPower;
        }
    }
}