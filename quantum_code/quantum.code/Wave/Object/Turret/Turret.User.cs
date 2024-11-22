using Photon.Deterministic;

namespace Quantum
{
    public unsafe partial struct Turret
    {
        public static void InteractTurret(Frame f, Transform3D turretTran, EntityRef turretEntity, EntityRef playerEntity)
        {
            if (!f.Unsafe.TryGetPointer(turretEntity, out Vehicle* vehicle)) return;
            if(vehicle->VehicleEntityRef != turretEntity) return;
            
            if (vehicle->RiderEntityRef == playerEntity)
            {
                ReleaseTurret(f, turretEntity, playerEntity);
                return;
            }
            if (vehicle->RiderEntityRef != EntityRef.None) return;

            UseTurret(f, turretEntity, playerEntity);
        }
        
        public static void SetConfig(Frame f, EntityRef turretEntity, Turret* turret)
        {
            if (f.Unsafe.TryGetPointer(turretEntity, out Interactor* interactor)) interactor->CanInteract = true;
            //if (!f.Unsafe.TryGetPointer(turretEntity, out Vehicle* vehicle)) return;
        }

        private static void UseTurret(Frame f, EntityRef turretEntity, EntityRef playerEntity)
        {
            if (!f.TryGet(turretEntity, out Transform3D turretTran)) return;
            if (!f.Unsafe.TryGetPointer(turretEntity , out Vehicle* vehicle)) return;
            if (!f.Unsafe.TryGetPointer(playerEntity, out Rider* rider)) return;
            vehicle->RiderEntityRef = playerEntity;
            rider->IsRiding = true;
            rider->VehicleEntity = turretEntity;
            f.Signals.OnPlayerUseTurretAnimStart(turretTran.Position, turretTran.Rotation, playerEntity);
        }

        private static void ReleaseTurret(Frame f, EntityRef turretEntity, EntityRef playerEntity)
        {
            if (!f.Unsafe.TryGetPointer(turretEntity, out Vehicle* vehicle)) return;
            if (!f.Unsafe.TryGetPointer(playerEntity, out Rider* rider)) return;
            rider->IsRiding = false;
            rider->VehicleEntity = EntityRef.None;
            vehicle->RiderEntityRef = EntityRef.None;
            Log.Info("Release Turret is Player:" + playerEntity.Index);
        }
    }
}