namespace Quantum.Wave.Object
{
    public unsafe class TurretSystem : SystemMainThreadFilter<TurretSystem.TurretFilter>
    {
        public struct TurretFilter
        {
            public EntityRef Entity;
            public Turret* Turret;
        }

        public override void Update(Frame f, ref TurretFilter filter)
        {
        }
    }
}