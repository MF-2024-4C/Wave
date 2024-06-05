namespace Quantum.Wave.Weapon;

public unsafe class WeaponChanger : SystemMainThreadFilter<WeaponInventorySystem.GunHolderFilter>
{
    public override void Update(Frame frame, ref WeaponInventorySystem.GunHolderFilter filter)
    {
        if (!frame.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* player)) return;

        Input input = *frame.GetPlayerInput(player->Player);

        TryChangeWeapon(frame, ref filter, player, input.ChangePrimaryWeapon, WeaponType.Primary);
        TryChangeWeapon(frame, ref filter, player, input.ChangeSecondaryWeapon, WeaponType.Secondary);
        TryChangeWeapon(frame, ref filter, player, input.ChangeTertiaryWeapon, WeaponType.Tertiary);
    }

    private void TryChangeWeapon(Frame frame, ref WeaponInventorySystem.GunHolderFilter filter, PlayerLink* player,
        bool changeWeaponCondition, WeaponType weaponType)
    {
        if (changeWeaponCondition &&
            filter.Inventory->GetWeaponFromType(frame, weaponType)->data != null &&
            filter.Inventory->currentWeaponType != weaponType)
        {
            var oldWeapon = filter.Inventory->GetCurrentWeapon(frame);
            frame.FindAsset<WeaponData>(oldWeapon->data.Id).OnUnEquip(oldWeapon);

            ChangeWeapon(frame, ref filter, player, weaponType);
            
            var newWeapon = filter.Inventory->GetWeaponFromType(frame, weaponType);
            frame.FindAsset<WeaponData>(newWeapon->data.Id).OnEquip(newWeapon);
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