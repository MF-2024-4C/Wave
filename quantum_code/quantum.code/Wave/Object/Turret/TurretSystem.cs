namespace Quantum
{
    public unsafe class TurretSystem : SystemMainThreadFilter<TurretSystem.TurretFilter>, ISignalOnInteractCall, ISignalOnComponentAdded<Turret>
    {
        public struct TurretFilter
        {
            public EntityRef Entity;
            public Turret* Turret;
        }

        public override void Update(Frame f, ref TurretFilter filter)
        {
        }

        public void OnInteractCall(Frame f, EntityRef interactor, EntityRef player)
        {
            Turret.InteractTurret(f, interactor, player);
        }

        private bool CheckTurretInteract(Frame f, EntityRef turret, EntityRef player)
        {
            if (!f.Unsafe.TryGetPointer<Interactor>(turret, out var interactor)) return false;
            interactor->OnInteract = true;
            return true;
        }

        public void OnAdded(Frame f, EntityRef entity, Turret* component)
        {
            Turret.SetConfig(f, entity, component);
        }
    }
}