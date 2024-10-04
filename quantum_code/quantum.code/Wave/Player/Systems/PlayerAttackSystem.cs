using Photon.Deterministic;

namespace Quantum.Player;

public class PlayerAttackSystem : SystemSignalsOnly, ISignalOnDamage
{
    public void OnDamage(Frame f, EntityRef target, DamageSource source, FP amount)
    {
        var causing = source.Causing;
        if (f.TryGet(causing, out PlayerLink player))
        {
            f.Events.OnPlayerAttackHitLocal(player.Player, source, amount);
        }
    }
}