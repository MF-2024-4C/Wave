namespace Quantum.Wave.Weapon;

public unsafe class WeaponChanger : SystemMainThreadFilter<WeaponInventorySystem.GunHolderFilter>
{
    public override void Update(Frame frame, ref WeaponInventorySystem.GunHolderFilter filter)
    {
        if (!frame.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* player)) return;

        var input = *frame.GetPlayerInput(player->Player);

        TryChangeWeapon(frame, ref filter, player, input.ChangePrimaryWeapon, WeaponType.Primary);
        TryChangeWeapon(frame, ref filter, player, input.ChangeSecondaryWeapon, WeaponType.Secondary);
        TryChangeWeapon(frame, ref filter, player, input.ChangeTertiaryWeapon, WeaponType.Tertiary);
    }

    private void TryChangeWeapon(Frame frame, ref WeaponInventorySystem.GunHolderFilter filter, PlayerLink* player,
        bool changeWeaponCondition, WeaponType weaponType)
    {
        if (!changeWeaponCondition) return;
        var weaponData = filter.Inventory->GetWeaponFromType(frame, weaponType);
        if (weaponData == null) return;
        if (weaponData->data == null) return;
        if (filter.Inventory->currentWeaponType == weaponType) return;

        var oldWeapon = filter.Inventory->GetCurrentWeapon(frame);
        frame.FindAsset<WeaponData>(oldWeapon->data.Id).OnUnEquip(oldWeapon);

        ChangeWeapon(frame, ref filter, player, weaponType);

        var newWeapon = filter.Inventory->GetWeaponFromType(frame, weaponType);
        var newWeaponEntity = filter.Inventory->GetCurrentWeaponEntity();
        frame.FindAsset<WeaponData>(newWeapon->data.Id).OnEquip(frame,player,newWeapon,newWeaponEntity);
    }


    private void ChangeWeapon(Frame frame, ref WeaponInventorySystem.GunHolderFilter filter, PlayerLink* player,
        WeaponType weaponType)
    {
        var weaponData = filter.Inventory->GetWeaponEntityFromType(weaponType);

        frame.Events.ChangeActiveWeapon(player->Player, weaponData);

        filter.Inventory->currentWeaponType = weaponType;
    }
}