namespace Quantum.Wave.Weapon;

public unsafe class WeaponChanger : SystemMainThreadFilter<WeaponInventorySystem.GunHolderFilter>
{
    public override void Update(Frame frame, ref WeaponInventorySystem.GunHolderFilter filter)
    {
        Input input = default;
        if (!frame.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* player)) return;

        input = *frame.GetPlayerInput(player->Player);

        if (input.ChangePrimaryWeapon && filter.Inventory->GetWeaponFromType(frame, WeaponType.Primary)->data != null)
        {
            ChangeWeapon(frame, ref filter, player, WeaponType.Primary);
        }

        if (input.ChangeSecondaryWeapon && filter.Inventory->GetWeaponFromType(frame, WeaponType.Secondary)->data != null)
        {
            ChangeWeapon(frame, ref filter, player, WeaponType.Secondary);
        }

        if (input.ChangeTertiaryWeapon && filter.Inventory->GetWeaponFromType(frame, WeaponType.Tertiary)->data != null)
        {
            ChangeWeapon(frame, ref filter, player, WeaponType.Tertiary);
        }
    }

    private void ChangeWeapon(Frame frame, ref WeaponInventorySystem.GunHolderFilter filter, PlayerLink* player,
        WeaponType weaponType)
    {
        frame.Events.ChangeActiveWeapon(player->Player,
            filter.Inventory->GetWeaponFromType(frame, weaponType)->data);

        filter.Inventory->currentWeaponType = weaponType;
    }
}