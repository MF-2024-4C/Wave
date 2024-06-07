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
                playerSys->SetHoldInteract(false, 0,EntityRef.None);
                return;
            }
            
            //インタラクト可能範囲にあるオブジェクトを取得してソート
            PlayerConfig config = f.FindAsset<PlayerConfig>(playerSys->Config.Id);
            var hits = playerSys->GetInteractRangeObject(f, transform->Position, config, input.CameraForwardDirection);
            hits.Sort(transform->Position);
            
            //インタラクトが可能なオブジェクトがあるか検索
            if (!playerSys->CheckNearInteractor(f, hits, out EntityRef hitEntity, out Interactor* hitInteractor))
            {
                playerSys->SetHoldInteract(false, 0,EntityRef.None);
                return;
            }

            InteractConfig iconfig = f.FindAsset<InteractConfig>(hitInteractor->Config.Id);
            if (iconfig.IsHoldInteract)
            {
                //現在インタラクトしているオブジェクトがあるかチェック
                if(playerSys->InteractEntity != EntityRef.None && playerSys->InteractEntity != hitEntity)
                {
                    //TODO::インタラクトしているオブジェクトが現在インタラクトされたものか確認
                    playerSys->InteractTime = 0;
                }
            }
            else
            {
                   //hitInteractor->OnInteract(f, entity, hitEntity);
                   Interactor.Interact(f, entity, hitEntity, hitInteractor);
                   playerSys->SetHoldInteract(false, 0,EntityRef.None);
            }
        }

        /// <summary>
        /// インタラクト可能範囲にあるすべてのオブジェクトを返す
        /// </summary>
        /// <param name="f">Frame</param>
        /// <param name="startPos">レイのスタート地点</param>
        /// <param name="config">プレイヤーコンフィグ</param>
        /// <param name="cameraForwardDir">カメラの前方向</param>
        /// <returns>範囲内にあるオブジェクト</returns>
        private HitCollection3D GetInteractRangeObject(Frame f, FPVector3 startPos, PlayerConfig config, FPVector3 cameraForwardDir)
        {
            var start = startPos + config.InteractRayOffset;
            var end = start + cameraForwardDir * config.InteractRayDistance;
            return f.Physics3D.LinecastAll(start, end);
        }

        /// <summary>
        /// 一番近いInteractorを取得
        /// </summary>
        /// <param name="f">Frame</param>
        /// <param name="hits">ヒットしたオブジェクト</param>
        /// <returns>Interactorが存在したかどうか</returns>
        private bool CheckNearInteractor(Frame f, HitCollection3D hits, out EntityRef hitEntity, out Interactor* retInteractor)
        {
            retInteractor = null;
            hitEntity = EntityRef.None;
            for (int i = 0; i < hits.Count; i++)
            {
                var hit = hits[i];
                if (f.Unsafe.TryGetPointer(hit.Entity, out Interactor* interactor))
                {
                    //if (!interacter->IsInteract) continue;
                    //Interactor.Interact(f, entity, hit.Entity, interactor);
                    retInteractor = interactor;
                    hitEntity = hit.Entity;
                    return true;
                }
            }
            return false;
        }
        
        private void SetHoldInteract(bool isHoldInteract, float interactTime,EntityRef entity)
        {
            IsHoldInteract = isHoldInteract;
            InteractTime = FP.FromFloat_UNSAFE(interactTime);
            InteractEntity = entity;
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