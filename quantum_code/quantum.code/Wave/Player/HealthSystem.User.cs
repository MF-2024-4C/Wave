using Photon.Deterministic;
namespace Quantum;

public unsafe partial struct HealthComponent
{
    public static void InitializeHealth(Frame f,HealthComponent* healthComponent)
    {
        healthComponent->CurrentHealth = healthComponent->MaxHealth;
    }

    public static void AddHealth(Frame f, EntityRef entity, HealthComponent* healthComponent, FP addHealth)
    {
        healthComponent->CurrentHealth += addHealth;
        if (healthComponent->CurrentHealth > healthComponent->MaxHealth)
            healthComponent->CurrentHealth = healthComponent->MaxHealth;
        
        //Log.Info($"Player{entity.Index} Health: {healthComponent->CurrentHealth}");
    }
    
    public static void DecHealth(Frame f, EntityRef entity, HealthComponent* healthComponent, FP decHealth)
    {
        healthComponent->CurrentHealth -= decHealth;
        if (healthComponent->CurrentHealth <= 0)
        {
            healthComponent->CurrentHealth = 0;
            f.Signals.OnDead(entity);
        }
        
        //Log.Info($"Player{entity.Index} Health: {healthComponent->CurrentHealth}");
    }
}