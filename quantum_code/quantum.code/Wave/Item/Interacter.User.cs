using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Photon.Deterministic;
using Quantum.Physics3D;

namespace Quantum
{
    public unsafe partial struct Interacter
    {
        public static void Interact(Frame f, EntityRef playerEntity, Interacter* interacter)
        {
            //if (!interacter->IsInteract) return;
            //Log.Info($"{playerEntity.} has Interact :")
            Log.Info($"player[{playerEntity.Index}] has Interact : [{interacter->InteractEntity.Index}]");
        }
    }
}