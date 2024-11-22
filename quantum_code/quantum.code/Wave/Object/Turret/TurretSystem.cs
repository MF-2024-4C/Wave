using System.Diagnostics;

namespace Quantum
{
    public unsafe partial class TurretSystem : SystemMainThreadFilter<TurretSystem.TurretFilter>, ISignalOnInteractCall, ISignalOnComponentAdded<Turret>, ISignalOnComponentAdded<Vehicle>, ISignalOnPlayerUseTurretAnimEnd
    {
        public struct TurretFilter
        {
            public EntityRef Entity;
            public Transform3D* Transform;
            public Turret* Turret;
            public Vehicle* Vehicle;
            public Interactor* Interactor;
        }

        public override void Update(Frame f, ref TurretFilter filter)
        {
            Log.Info($"Interact Cool Time: {filter.Interactor->NowCoolDown}");
        }
        public void OnAdded(Frame f, EntityRef entity, Turret* component)
        {
            Log.Info("OnAddedTurret");
            Turret.SetConfig(f, entity, component);
        }
        
        public void OnAdded(Frame f, EntityRef entity, Vehicle* component)
        {
            component->VehicleEntityRef = entity;
            component->RiderEntityRef = EntityRef.None;
        }

        public void OnPlayerUseTurretAnimEnd(Frame f)
        {
            Log.Info("OnPlayerUseTurretAnimEnd");
        }

        public void OnInteractCall(Frame f, EntityRef interactor, EntityRef player)
        { 
            if (!f.TryGet<Transform3D>(interactor, out var transform)) return;
            Turret.InteractTurret(f, transform, interactor, player);
        }
    }
}