using System.Diagnostics;

namespace Quantum
{
    public unsafe partial class TurretSystem : SystemMainThreadFilter<TurretSystem.TurretFilter>, ISignalOnInteractCall, ISignalOnComponentAdded<Turret>, ISignalOnPlayerUseTurretAnimEnd
    {
        public struct TurretFilter
        {
            public EntityRef Entity;
            public Transform3D* Transform;
            public Turret* Turret;
        }

        public override void Update(Frame f, ref TurretFilter filter)
        {
        }
        public void OnAdded(Frame f, EntityRef entity, Turret* component)
        {
            Log.Info("OnAddedTurret");
            Turret.SetConfig(f, entity, component);
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