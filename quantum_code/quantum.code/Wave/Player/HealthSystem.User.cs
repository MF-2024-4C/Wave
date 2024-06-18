using Photon.Deterministic;
namespace Quantum;

public unsafe partial struct HealthComponent
{
    public static void InitializeHealth(Frame f,HealthComponent* healthComponent)
    {
        if (!f.TryFindAsset(healthComponent->Config.Id, out HealthConfig config))
        {
            Log.Info("HealthConfig not found");
            return;
        }
        
        healthComponent->CurrentHealth = config.MaxHealth;
    }

    public static void AddHealth(Frame f, EntityRef entity, HealthComponent* healthComponent, FP addHealth)
    {
        var config = f.FindAsset<HealthConfig>(healthComponent->Config.Id);
        healthComponent->CurrentHealth += addHealth;
        if (healthComponent->CurrentHealth > config.MaxHealth)
            healthComponent->CurrentHealth = config.MaxHealth;
        
        Log.Info($"Player{entity.Index} Health: {healthComponent->CurrentHealth}");
    }
    
    public static void DecHealth(Frame f, EntityRef entity, HealthComponent* healthComponent, FP decHealth)
    {
        healthComponent->CurrentHealth -= decHealth;
        if (healthComponent->CurrentHealth <= 0)
        {
            healthComponent->CurrentHealth = 0;
            f.Signals.OnDead(entity);
        }
        
        Log.Info($"Player{entity.Index} Health: {healthComponent->CurrentHealth}");
    }
}