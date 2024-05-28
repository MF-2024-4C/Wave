namespace Quantum.Wave.Weapon;

public unsafe class WeaponChanger : SystemMainThreadFilter<WeaponInventorySystem.GunHolderFilter>
{
    public override void Update(Frame frame, ref WeaponInventorySystem.GunHolderFilter filter)
    {
        Input input = default;
        if (!frame.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* player)) return;

        input = *frame.GetPlayerInput(player->Player);

        if (input.ChangePrimaryWeapon && filter.Inventory->PrimaryWeaponData != null)
        {
            frame.Events.ChangeActiveWeapon(player->Player,filter.Inventory->PrimaryWeaponData);
        }

        if (input.ChangeSecondaryWeapon && filter.Inventory->SecondaryWeaponData != null)
        {
            frame.Events.ChangeActiveWeapon(player->Player,filter.Inventory->SecondaryWeaponData);
        }

        if (input.ChangeTertiaryWeapon && filter.Inventory->TertiaryWeaponData != null)
        {
            frame.Events.ChangeActiveWeapon(player->Player,filter.Inventory->TertiaryWeaponData);
        }
    }
}