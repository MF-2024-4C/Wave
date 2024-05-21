namespace Quantum.Wave
{
    public unsafe class InteractSystem : SystemMainThreadFilter<InteractSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public Transform3D* Transform;
        }

        public override void Update(Frame f, ref Filter filter)
        {
        }
    }
}