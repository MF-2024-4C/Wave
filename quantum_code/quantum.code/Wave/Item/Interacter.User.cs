using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Photon.Deterministic;
using Quantum.Physics3D;

namespace Quantum
{
    public unsafe partial struct Interactor
    {
        public static void Interact(Frame f, EntityRef playerEntity, EntityRef interactorEntity, Interactor* interactor)
        {
            //if (!interacter->IsInteract) return;
            //Log.Info($"{playerEntity.} has Interact :")
            //Log.Info($"player[{playerEntity.Index}] has Interact : [{interactorEntity.Index}]");

            //コンフィグの取得
            if (!f.TryFindAsset<InteractConfig>(interactor->Config.Id, out var interactConfig))
            {
                Log.Error("InteractConfig is not found");
                return;
            }

            //インタラクトができるかどうか
            if (!interactor->CanInteract) return;
            
            //現在インタラクト中かどうか
            if (interactor->OnInteract) return;
            
            //クールタイムの確認
            var time = f.Number * f.DeltaTime;
            if(interactor->NowCoolDown) return;
            
            //インタラクトの種類によって処理を分ける
            if (interactConfig.IsHoldInteract)
            {
                //すでにインタラクト中の場合は何もしない
                if (interactor->OnInteract)
                {
                    Log.Info($"Interactor[{interactorEntity.Index}] is already Interacting");
                    return;
                }
                
                interactor->OnInteract = true;
                interactor->InteractStartTime = time;
                interactor->InteractPlayer = playerEntity;
            }
            else
            {
                f.Signals.OnInteractCall(interactorEntity, playerEntity);
                interactor->OnInteract = false;
                interactor->InteractEndTime = time;
            }
        }

        public static void Release(Frame f, EntityRef playerEntity, Interactor* interactor)
        {
            if (!interactor->OnInteract) return;
            if(interactor->InteractPlayer != playerEntity) return;
            //Log.Info($"player[{playerEntity.Index}] has Release Interact : [{interactorEntity.Index}]");
            interactor->OnInteract = false;
            interactor->InteractEndTime = f.Number * f.DeltaTime;
            interactor->InteractPlayer = EntityRef.None;
        }

        public static void CheckElapsedTime(Frame f, EntityRef interactorEntity, Interactor* interactor)
        {
            if (!f.TryFindAsset<InteractConfig>(interactor->Config.Id, out var interactConfig)) return;
            if (!interactConfig.IsHoldInteract) return;
            if (!interactor->OnInteract) return;
            
            if (f.Number * f.DeltaTime - interactor->InteractStartTime > interactConfig.HoldTime)
            {
                f.Signals.OnInteractCall(interactorEntity, interactor->InteractPlayer);
                interactor->OnInteract = false;
                interactor->InteractPlayer = EntityRef.None;
                interactor->InteractEndTime = f.Number * f.DeltaTime;
            }
        }

        public static void CheckInteractCoolTime(Frame f, Interactor* interactor)
        {
            if (!f.TryFindAsset<InteractConfig>(interactor->Config.Id, out var interactConfig)) return;
            
            var time = f.Number * f.DeltaTime;
            if (time - interactor->InteractEndTime < interactConfig.InteractCoolTime) interactor->NowCoolDown = true;
            else interactor->NowCoolDown = false;
        }
    }
}