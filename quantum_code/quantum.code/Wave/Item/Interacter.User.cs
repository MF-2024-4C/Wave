using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Photon.Deterministic;
using Quantum.Physics3D;

namespace Quantum
{
    public unsafe partial struct Interacter
    {
        public static void Interact(Frame f, EntityRef interacterEntity, Interacter* interacter, Transform3D* transform)
        {
            if(interacter->IsInteract)
            {
                Log.Info(interacterEntity.ToString() + "is Interact has " + interacter->IsInteract);
                //f.Physics3D.Raycast()
            }
        }
    }
}