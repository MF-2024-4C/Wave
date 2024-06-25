using System.Runtime.CompilerServices;
using Quantum.QuantumDemo;
using Photon.Deterministic;
using Quantum.Physics2D;
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
            playerSys->CameraForwardDirection = input.CameraForwardDirection;
        }

        public static void Interact(Frame f, EntityRef entity, Transform3D* transform, PlayerSys* playerSys,
            Input input)
        {
            //インプットを入力しているか確認
            if (!input.Interact)
            {
                if (playerSys->InteractEntity == EntityRef.None) return;
                f.Signals.OnReleaseInteractor(playerSys->InteractEntity, entity);
                playerSys->InteractEntity = EntityRef.None;
                return;
            }
            
            //インタラクト可能範囲にあるオブジェクトを取得してソート
            PlayerConfig config = f.FindAsset<PlayerConfig>(playerSys->Config.Id);
            var start = transform->Position + config.InteractRayOffset;
            var end = start + input.CameraForwardDirection * config.InteractRayDistance;
            var hits = f.Physics3D.LinecastAll(start, end);
            hits.Sort(transform->Position);
            
            //一番近いInteractorを取得してインタラクト
            for (int i = 0; i < hits.Count; i++)
            {
                var hit = hits[i];
                if (f.Unsafe.TryGetPointer(hit.Entity, out Interactor* interactor))
                {
                    Interactor.Interact(f, entity, hit.Entity, interactor);
                    playerSys->InteractEntity = hit.Entity;
                    return;
                }
            }
        }
        
        public static void Dead(Frame f, EntityRef entity, PlayerSys* playerSys)
        {
            playerSys->Dead(entity);
        }

        public static void Recoil(FPVector2 recoil)
        {
        }

        public void SetConfig(Frame　f, EntityRef entity)
        {
            PlayerConfig config = f.FindAsset<PlayerConfig>(this.Config.Id);
            WalkSpeed = config.WalkSpeed;
            RunSpeed = config.RunSpeed;
            JumpPower = config.JumpPower;
            BreakPower = config.BreakPower;

            if (!f.Unsafe.TryGetPointer(entity, out HealthComponent* health)) return;
            //if (!f.Unsafe.TryGetPointer(entity, out PlayerLink* playerLink)) return;
            //f.Events.PlayerSpawnEvent(entity);
            //Log.Info("イベント発行");
            HealthComponent.InitializeHealth(f, health);
        }

        private void Dead(EntityRef entity)
        {
            Log.Info($"Player{entity.Index} is Dead");
        }
    }
}