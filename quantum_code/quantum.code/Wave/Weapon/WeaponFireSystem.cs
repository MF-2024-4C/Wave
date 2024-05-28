using Quantum.Player;

namespace Quantum.Wave.Weapon;

public unsafe class WeaponFireSystem : SystemMainThreadFilter<PlayerSystem.PlayerFilter>
{
    public override void Update(Frame frame, ref PlayerSystem.PlayerFilter filter)
    {
        Input input = default;
        if (!frame.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* player)) return;

        input = *frame.GetPlayerInput(player->Player);

        if (input.Fire.WasPressed)
        {
            frame.Events.Fire(player->Player);
        }
    }
}