using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Photon.Deterministic;

namespace Quantum
{
    public unsafe partial struct Interacter
    {
        public static void Interact(Frame f, EntityRef interacterEntity, Interacter* interacter)
        {
            if(interacter->IsInteract)
            {
                Log.Info(interacterEntity.ToString() + "is Interact has " + interacter->IsInteract);
            }
        }
    }
}