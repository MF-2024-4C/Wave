namespace Quantum.Wave
{
    public unsafe class InteractSystem : SystemMainThreadFilter<InteractSystem.Filter> , ISignalOnComponentAdded<Interacter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public Transform3D* Transform;
            public Interacter* Interacter;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            //Interacter.Interact(f, filter.Entity, filter.Interacter);
        }

        public void OnAdded(Frame f, EntityRef entity, Interacter* component)
        {
            
        }
    }
}