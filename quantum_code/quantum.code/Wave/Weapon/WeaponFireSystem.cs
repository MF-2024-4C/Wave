namespace Quantum.Wave.Weapon;

public unsafe class WeaponFireSystem : SystemMainThreadFilter<WeaponInventorySystem.GunHolderFilter>
{
    public override void Update(Frame frame, ref WeaponInventorySystem.GunHolderFilter filter)
    {
        if (!frame.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* player)) return;
        
        var input = *frame.GetPlayerInput(player->Player);

        var currentWeapon = filter.Inventory->GetCurrentWeapon(frame);

        Fire(frame, player, input, currentWeapon);

        Reload(frame, player, input, currentWeapon);

        RecoilProgress(frame, currentWeapon);
    }

    private void Fire(Frame frame, PlayerLink* player, Input input, Quantum.Weapon* currentWeapon)
    {
        FireProgress(frame, currentWeapon);

        if (!currentWeapon->CanFire()) return;

        var shouldFire = currentWeapon->currentFireMode switch
        {
            FireMode.FullAuto => input.Fire.IsDown,
            FireMode.SemiAuto => input.Fire.WasPressed,
            _ => false
        };

        if (shouldFire)
            currentWeapon->Fire(frame, player);
    }

    private void FireProgress(Frame frame, Quantum.Weapon* currentWeapon)
    {
        if (currentWeapon->nextFireTime > 0)
            currentWeapon->nextFireTime -= frame.DeltaTime;
    }

    private void Reload(Frame frame, PlayerLink* player, Input input, Quantum.Weapon* currentWeapon)
    {
        ReloadProgress(frame, currentWeapon);

        if (input.Reload.WasPressed && currentWeapon->CanReload())
        {
            currentWeapon->Reload(frame, player);
        }
    }

    private void ReloadProgress(Frame frame, Quantum.Weapon* currentWeapon)
    {
        if (!currentWeapon->IsReloading()) return;

        if (currentWeapon->reloadingTime > 0)
        {
            currentWeapon->reloadingTime -= frame.DeltaTime;
        }
        else
        {
            currentWeapon->OnReloaded();
        }
    }

    private void RecoilProgress(Frame frame, Quantum.Weapon* currentWeapon)
    {
        if (currentWeapon->nextFireTime > 0 || currentWeapon->recoilProgressTime <= 0) return;

        currentWeapon->recoilProgressTime -= 1 / currentWeapon->recoilProgressRate * frame.DeltaTime;

        if (currentWeapon->recoilProgressTime < 0)
            currentWeapon->recoilProgressTime = 0;
    }
}