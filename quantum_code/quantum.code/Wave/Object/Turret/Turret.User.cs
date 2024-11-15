using Photon.Deterministic;

namespace Quantum
{
    public unsafe partial struct Turret
    {
        public static void InteractTurret(Frame f, Transform3D turretTran, EntityRef turretEntity, EntityRef playerEntity)
        {
            if(!f.Unsafe.TryGetPointer<Turret>(turretEntity, out Turret* turret)) return;
            if(turret->TurretEntity != turretEntity) return;
            if (turret->UsePlayer == playerEntity)
            {
                ReleaseTurret(f, turret, playerEntity);
                return;
            }
            if (turret->UsePlayer != EntityRef.None) return;

            UseTurret(f, turret, playerEntity);
        }
        
        public static void SetConfig(Frame f, EntityRef turretEntity, Turret* turret)
        {
            turret->TurretEntity = turretEntity;
            turret->UsePlayer = EntityRef.None;
            if (f.Unsafe.TryGetPointer(turretEntity, out Interactor* interactor)) interactor->CanInteract = true;
        }

        private static void UseTurret(Frame f, Turret* turret, EntityRef playerEntity)
        {
            if (!f.TryGet(turret->TurretEntity, out Transform3D turretTran)) return;
            turret->UsePlayer = playerEntity;
            f.Signals.OnPlayerUseTurretAnimStart(turretTran.Position, turretTran.Rotation, playerEntity);
        }

        private static void ReleaseTurret(Frame f, Turret* turret, EntityRef playerEntity)
        {
            turret->UsePlayer = EntityRef.None;
            Log.Info("Release Turret is Player:" + playerEntity.Index);
        }
    }
}