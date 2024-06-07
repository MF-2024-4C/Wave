using System;
using Photon.Deterministic;

namespace Quantum
{
    public abstract unsafe partial class BaseItemConfig
    {
        public FP TestFP;

        public abstract void Execute(Frame f, EntityRef item, EntityRef player);
        public abstract void Release(Frame f, EntityRef item, EntityRef player);
    }
    
    [Serializable]
    public unsafe partial class Item1Config : BaseItemConfig
    {
        public FP TestFP1;

        public override void Execute(Frame f, EntityRef item, EntityRef player)
        {
            if (f.Unsafe.TryGetPointer<Transform3D>(player, out Transform3D* pTran))
            {
                pTran->Position.Y += TestFP1;
            }
        }

        public override void Release(Frame f, EntityRef item, EntityRef player)
        {
        }
    }

    [Serializable]
    public  unsafe partial class Item2Config : BaseItemConfig
    {
        public FP TestFP2;
        
        public override void Execute(Frame f, EntityRef item, EntityRef player)
        {
            if (f.Unsafe.TryGetPointer<Transform3D>(player, out Transform3D* pTran))
            {
                pTran->Position.X += TestFP2;
            }
        }

        public override void Release(Frame f, EntityRef item, EntityRef player)
        {
        }
    }
    
    [Serializable]
    public unsafe partial class Item3Config : BaseItemConfig
    {
        
        public FP TestFP3;
        public override void Execute(Frame f, EntityRef item, EntityRef player)
        {
            if (f.Unsafe.TryGetPointer<Transform3D>(player, out Transform3D* pTran))
            {
                pTran->Position.X -= TestFP3;
            }
        }

        public override void Release(Frame f, EntityRef item, EntityRef player)
        {
            Log.Info("Item3 is Cancelled");
        }
    }
}