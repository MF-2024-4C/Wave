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
            if (playerSys->IsDead) return;
            
            bool isJump = false;
            //ジャンプ処理
            if (input.PlayerJump.WasPressed && controller->Grounded)
            {
                controller->Jump(f, false, playerSys->JumpPower);
                isJump = true;
            }
            
            //移動処理
            FP speed = playerSys->WalkSpeed;
            FPVector3 dir = input.PlayerDirection.XOY;
            //ダッシュ処理
            if (input.PlayerDash)
            {
                speed = playerSys->RunSpeed;
            }
            
            //地面にいるとき
            if (controller->Grounded && !isJump)
            {
                dir = input.PlayerDirection.XOY;
            }
            else
            {
                //空中にいて移動入力がないとき
                if (input.PlayerDirection == FPVector2.Zero)
                {
                    FPVector3 velo = controller->Velocity;
                    velo.Y = 0;
                    dir = velo.Normalized;
                }
            }
            
            controller->MaxSpeed = speed;
            controller->Move(f, entityRef, dir);
            playerSys->PlayerAnimState = GetAnimState(input, controller->Grounded, isJump);
        }

        public static void Rot(Frame f, EntityRef entity, Transform3D* transform, CharacterController3D* controller,
            PlayerSys* playerSys, Input input)
        {
            //カメラの向いている方向にプレイヤーも回転する
            FPQuaternion targetRotation = FPQuaternion.LookRotation(input.CameraForwardDirection);
            targetRotation.X = 0;
            targetRotation.Z = 0;
            playerSys->TargetRotation = targetRotation;

            //インタラクト用にカメラの向いている方向を保存
            playerSys->CameraForwardDirection = input.CameraForwardDirection;
        }

        public static void Interact(Frame f, EntityRef entity, Transform3D* transform, PlayerSys* playerSys,
            Input input)
        {
            if (playerSys->IsDead) return;
            
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
            var start = transform->Position + playerSys->InteractRayOffset;
            var end = start + input.CameraForwardDirection * playerSys->InteractRayDistance;
            var hits = f.Physics3D.LinecastAll(start, end);
            hits.Sort(transform->Position);
            
            //一番近いInteractorを取得してインタラクト
            for (int i = 0; i < hits.Count; i++)
            {
                var hit = hits[i];
                if (f.Unsafe.TryGetPointer(hit.Entity, out Interactor* interactor))
                {
                    //自分の場合はインタラクトしない
                    if (hit.Entity == entity) continue;
                    
                    Interactor.Interact(f, entity, hit.Entity, interactor);
                    playerSys->InteractEntity = hit.Entity;
                    return;
                }
            }
        }
        
        public static void Dead(Frame f, EntityRef entity, PlayerSys* playerSys)
        {
            Log.Info($"Player{entity.Index} is Dead");

            if (!f.Unsafe.TryGetPointer<Interactor>(entity, out Interactor* interactor)) return;
            interactor->CanInteract = true;

            playerSys->IsDead = true;
            playerSys->PlayerAnimState = PlayerConfig.PAnimDown;
        }

        public static void Recoil(FPVector2 recoil)
        {
        }

        public static void Revive(Frame f, EntityRef entity)
        {
            if (!f.Unsafe.TryGetPointer(entity, out PlayerSys* playerSys)) return;
            if (!f.Unsafe.TryGetPointer(entity, out HealthComponent* healthComp)) return;
            if (!f.Unsafe.TryGetPointer(entity, out Interactor* interactor)) return;
            
            //TODO::蘇生時の体力をコンフィグで設定できるようにする
            HealthComponent.Revive(f, entity, healthComp, healthComp->MaxHealth);
            interactor->CanInteract = false;
            playerSys->IsDead = false;

            if (f.TryGet(entity, out PlayerLink playerLink))
            {
                f.Events.PlayerReviveEvent(playerLink);
            }
        }

        public void SetConfig(Frame　f, EntityRef entity)
        {
            PlayerConfig config = f.FindAsset<PlayerConfig>(this.Config.Id);
            WalkSpeed = config.WalkSpeed;
            RunSpeed = config.RunSpeed;
            JumpPower = config.JumpPower;
            AirMovePower = config.AirMovePower;

            if (!f.Unsafe.TryGetPointer(entity, out HealthComponent* health)) return;
            HealthComponent.InitializeHealth(f, health);

            if (!f.Unsafe.TryGetPointer(entity, out Interactor* interactor)) return;
            interactor->CanInteract = false;

            IsDead = false;
        }

        private static byte GetAnimState(Input input, bool isGround, bool isJumped)
        {
            byte animState = PlayerConfig.PAnimIdle;

            //ジャンプしたとき
            if (isJumped)
            {
                animState = PlayerConfig.PAnimJump;
                return animState;
            }

            //地面にいないとき
            if (!isGround)
            {
                animState = PlayerConfig.PAnimFall;
                return animState;
            }
            
            //移動していないとき
            if (input.PlayerDirection == FPVector2.Zero) return animState;
            //ダッシュしているとき
            if(input.PlayerDash) return PlayerConfig.PAnimRun;
            //歩いているとき
            animState = PlayerConfig.PAnimMove;
            return animState;
        }
    }
}