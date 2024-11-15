using Photon.Deterministic;

namespace Quantum
{
    public unsafe partial struct Turret
    {
        public static void InteractTurret(Frame f, EntityRef turretEntity, EntityRef playerEntity)
        {
            if(!f.Unsafe.TryGetPointer<Turret>(turretEntity, out Turret* turret)) return;
            if(turret->TurretEntity != turretEntity) return;
            if(turret->UsePlayer == EntityRef.None) return;

            UseTurret(f, turret, playerEntity);
        }
        
        public static void SetConfig(Frame f, EntityRef turretEntity, Turret* turret)
        {
            turret->TurretEntity = turretEntity;
            turret->UsePlayer = EntityRef.None;
        }

        private static void UseTurret(Frame f, Turret* turret, EntityRef playerEntity)
        {
            turret->UsePlayer = playerEntity;
        }

        private static void ReleaseTurret(Frame f, Turret* turret, EntityRef playerEntity)
        {
            turret->UsePlayer = EntityRef.None;
        }
    }
}