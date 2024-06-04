using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Photon.Deterministic;
using Quantum.Physics3D;

namespace Quantum
{
    public unsafe partial struct Interactor
    {
        public static void Interact(Frame f, EntityRef playerEntity, EntityRef itemEntity, Interactor* interacter)
        {
            //if (!interacter->IsInteract) return;
            //Log.Info($"{playerEntity.} has Interact :")
            Log.Info($"player[{playerEntity.Index}] has Interact : [{itemEntity.Index}]");
            f.Signals.OnInteract(itemEntity, playerEntity);
        }
    }
}