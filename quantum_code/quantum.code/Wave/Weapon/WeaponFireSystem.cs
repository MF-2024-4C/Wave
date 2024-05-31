namespace Quantum.Wave.Weapon;

public unsafe class WeaponFireSystem : SystemMainThreadFilter<WeaponInventorySystem.GunHolderFilter>
{
    public override void Update(Frame frame, ref WeaponInventorySystem.GunHolderFilter filter)
    {
        Input input = default;
        if (!frame.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* player)) return;

        input = *frame.GetPlayerInput(player->Player);

        filter.Inventory->GetCurrentWeapon(frame)->nextFireTime += frame.DeltaTime;

        if (filter.Inventory->GetCurrentWeapon(frame)->currentFireMode == FireMode.FullAuto)
        {
            FullAutoFire(frame, filter, input, player);
        }
        else if (filter.Inventory->GetCurrentWeapon(frame)->currentFireMode == FireMode.SemiAuto)
        {
            SemiAutoFire(frame, filter, input, player);
        }

        if (input.Reload.WasPressed && !filter.Inventory->GetCurrentWeapon(frame)->isReloading)
        {
            Reload(frame, filter, player);
        }
    }

    private void Reload(Frame frame, WeaponInventorySystem.GunHolderFilter filter, PlayerLink* player)
    {
        SendReloadEvent(frame, player);
        filter.Inventory->GetCurrentWeapon(frame)->currentAmmo = filter.Inventory->GetCurrentWeapon(frame)->maxAmmo;
    }

    private void FullAutoFire(Frame frame, WeaponInventorySystem.GunHolderFilter filter, Input input,
        PlayerLink* player)
    {
        if (!input.Fire.IsDown) return;
        if (!IsEndedFireCoolTime(frame, filter)) return;
        if (!IsExistAmmo(frame, filter)) return;

        SendFireEvent(frame, player);
        filter.Inventory->GetCurrentWeapon(frame)->currentAmmo--;
        filter.Inventory->GetCurrentWeapon(frame)->nextFireTime = 0;
    }

    private void SemiAutoFire(Frame frame, WeaponInventorySystem.GunHolderFilter filter, Input input,
        PlayerLink* player)
    {
        if (!input.Fire.WasPressed) return;
        if (!IsEndedFireCoolTime(frame, filter)) return;
        if (!IsExistAmmo(frame, filter)) return;

        SendFireEvent(frame, player);
        filter.Inventory->GetCurrentWeapon(frame)->currentAmmo--;
        filter.Inventory->GetCurrentWeapon(frame)->nextFireTime = 0;
    }

    private bool IsExistAmmo(Frame frame, WeaponInventorySystem.GunHolderFilter filter)
    {
        var ammo = filter.Inventory->GetCurrentWeapon(frame)->currentAmmo;
        return ammo > 0;
    }

    private bool IsEndedFireCoolTime(Frame frame, WeaponInventorySystem.GunHolderFilter filter)
    {
        var coolTime = filter.Inventory->GetCurrentWeapon(frame)->fireRate;
        return filter.Inventory->GetCurrentWeapon(frame)->nextFireTime >= coolTime;
    }

    private void SendFireEvent(Frame frame, PlayerLink* player)
    {
        frame.Events.Fire(player->Player);
    }

    private void SendReloadEvent(Frame frame, PlayerLink* player)
    {
        frame.Events.Reload(player->Player);
    }
}