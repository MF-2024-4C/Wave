namespace Quantum.Wave.Gun;

public unsafe class WeaponChanger : SystemMainThreadFilter<PlayerFilter>
{
    public override void Update(Frame frame, ref PlayerFilter filter)
    {
        Log.Info(" GunChanger SystemMainThreadFilter<PlayerFilter> Update");

        Input input = default;
        if (!frame.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* player)) return;

        input = *frame.GetPlayerInput(player->Player);

        if (input.ChangePrimaryWeapon)
        {
            frame.Events.ChangePrimaryWeapon(player->Player);
        }

        if (input.ChangeSecondaryWeapon)
        {
            frame.Events.ChangeSecondaryWeapon(player->Player);
        }

        if (input.ChangeTertiaryWeapon)
        {
            frame.Events.ChangeTertiaryWeapon(player->Player);
        }
    }
}