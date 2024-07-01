using Photon.Deterministic;

namespace Quantum.Player;

public unsafe class HealthSystem : SystemMainThreadFilter<HealthSystem.Filter>, ISignalOnAddHealth, ISignalOnDecHealth
{
    public struct Filter
    {
        public EntityRef Entity;
        public HealthComponent* Health;
    }

    public override void Update(Frame f, ref Filter filter)
    {
    }

    public void OnAddHealth(Frame f, EntityRef entity, FP amount)
    {
        if(!f.Unsafe.TryGetPointer(entity, out HealthComponent* healthComponent))
        {
            Log.Info("HealthComponent not found");
            return;
        }
        
        HealthComponent.AddHealth(f, entity, healthComponent, amount);
    }

    public void OnDecHealth(Frame f, EntityRef entity, FP amount)
    {
        if(!f.Unsafe.TryGetPointer(entity, out HealthComponent* healthComponent))
        {
            Log.Info("HealthComponent not found");
            return;
        }

        HealthComponent.DecHealth(f, entity, healthComponent, amount);
    }
}