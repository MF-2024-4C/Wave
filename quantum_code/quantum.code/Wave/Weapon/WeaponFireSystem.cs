namespace Quantum.Wave.Weapon;

public unsafe class WeaponFireSystem : SystemMainThreadFilter<WeaponInventorySystem.GunHolderFilter>
{
    Quantum.Weapon* currentWeapon;

    public override void Update(Frame frame, ref WeaponInventorySystem.GunHolderFilter filter)
    {
        Input input = default;
        if (!frame.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* player)) return;

        input = *frame.GetPlayerInput(player->Player);

        currentWeapon = filter.Inventory->GetCurrentWeapon(frame);


        //Fire
        //---------------------------------------------
        currentWeapon->nextFireTime += frame.DeltaTime;

        if (currentWeapon->currentFireMode == FireMode.FullAuto)
        {
            FullAutoFire(frame, filter, input, player);
        }
        else if (currentWeapon->currentFireMode == FireMode.SemiAuto)
        {
            SemiAutoFire(frame, filter, input, player);
        }
        //---------------------------------------------


        //Reload
        //---------------------------------------------
        if (currentWeapon->isReloading)
        {
            currentWeapon->reloadingTime += frame.DeltaTime;

            if (currentWeapon->reloadingTime >= currentWeapon->reloadTime) //リロード完了
            {
                currentWeapon->isReloading = false;
                currentWeapon->reloadingTime = 0;
            }
        }
        else if (input.Reload.WasPressed && //リロードキーが押された
                 !IsReloading() && //リロード中でない
                 currentWeapon->currentAmmo < currentWeapon->maxAmmo) //1発以上弾が減っている
        {
            Reload(frame, filter, player);
        }
        //---------------------------------------------
    }

    private void Reload(Frame frame, WeaponInventorySystem.GunHolderFilter filter, PlayerLink* player)
    {
        SendReloadEvent(frame, player);
        currentWeapon->currentAmmo = currentWeapon->maxAmmo;
        currentWeapon->isReloading = true;
    }

    private void FullAutoFire(Frame frame, WeaponInventorySystem.GunHolderFilter filter, Input input,
        PlayerLink* player)
    {
        if (!input.Fire.IsDown) return;
        if (!IsEndedFireCoolTime()) return;
        if (!IsExistAmmo()) return;
        if (IsReloading()) return;

        SendFireEvent(frame, player);
        currentWeapon->currentAmmo--;
        currentWeapon->nextFireTime = 0;
    }

    private void SemiAutoFire(Frame frame, WeaponInventorySystem.GunHolderFilter filter, Input input,
        PlayerLink* player)
    {
        if (!input.Fire.WasPressed) return;
        if (!IsEndedFireCoolTime()) return;
        if (!IsExistAmmo()) return;
        if (IsReloading()) return;

        SendFireEvent(frame, player);
        currentWeapon->currentAmmo--;
        currentWeapon->nextFireTime = 0;
    }

    private bool IsReloading()
    {
        return currentWeapon->isReloading;
    }

    private bool IsExistAmmo()
    {
        var ammo = currentWeapon->currentAmmo;
        return ammo > 0;
    }

    private bool IsEndedFireCoolTime()
    {
        var coolTime = currentWeapon->fireRate;
        return currentWeapon->nextFireTime >= coolTime;
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