﻿using System.Dynamic;

namespace Quantum.Wave.Item
{
    public unsafe partial class ItemSystem : SystemMainThreadFilter<ItemSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public Transform3D* Transform;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            throw new System.NotImplementedException();
        }

    }
}
