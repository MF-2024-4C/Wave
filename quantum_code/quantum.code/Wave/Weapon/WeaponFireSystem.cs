using Photon.Deterministic;

namespace Quantum.Wave.Weapon;

public unsafe class WeaponFireSystem : SystemMainThreadFilter<WeaponInventorySystem.GunHolderFilter>
{
    public override void Update(Frame frame, ref WeaponInventorySystem.GunHolderFilter filter)
    {
        if (!frame.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* player)) return;

        var input = *frame.GetPlayerInput(player->Player);

        var currentWeapon = filter.Inventory->GetCurrentWeapon(frame);

        EquipProgress(frame, currentWeapon);

        Fire(frame, ref filter, player, input, currentWeapon);

        Reload(frame, ref filter, player, input, currentWeapon);

        RecoilProgress(frame, currentWeapon);
    }

    private void EquipProgress(Frame frame, Quantum.Weapon* currentWeapon)
    {
        if (currentWeapon->equipTime > FP._0)
            currentWeapon->equipTime -= frame.DeltaTime;
    }

    private void Fire(Frame frame, ref WeaponInventorySystem.GunHolderFilter filter, PlayerLink* player, Input input,
        Quantum.Weapon* currentWeapon)
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
        {
            var weapon = filter.Inventory->GetCurrentWeaponEntity();
            currentWeapon->Fire(frame, player, weapon);
        }
    }

    private void FireProgress(Frame frame, Quantum.Weapon* currentWeapon)
    {
        if (currentWeapon->nextFireTime > FP._0)
            currentWeapon->nextFireTime -= frame.DeltaTime;
    }

    private void Reload(Frame frame, ref WeaponInventorySystem.GunHolderFilter filter, PlayerLink* player, Input input,
        Quantum.Weapon* currentWeapon)
    {
        ReloadProgress(frame, currentWeapon);

        if (input.Reload.WasPressed && currentWeapon->CanReload())
        {
            var weapon = filter.Inventory->GetCurrentWeaponEntity();
            currentWeapon->Reload(frame, player, weapon);
        }
    }

    private void ReloadProgress(Frame frame, Quantum.Weapon* currentWeapon)
    {
        if (!currentWeapon->IsReloading()) return;

        if (currentWeapon->reloadingTime > FP._0)
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
        if (currentWeapon->nextFireTime > FP._0 || currentWeapon->recoilProgressTime <= FP._0) return;

        currentWeapon->recoilProgressTime -= FP._1 / currentWeapon->recoilProgressRate * frame.DeltaTime;

        if (currentWeapon->recoilProgressTime < FP._0)
            currentWeapon->recoilProgressTime = FP._0;
    }
}