using Photon.Deterministic;

namespace Quantum;

public unsafe partial struct Weapon
{
    public void Fire(Frame frame, PlayerLink* player)
    {
        OnFire(frame, player);
    }

    private void OnFire(Frame frame, PlayerLink* player)
    {
        currentAmmo--;
        nextFireTime = FP._1 / fireRate;

        Recoil(frame);
        SendFireEvent(frame, player);
    }

    public void Reload(Frame frame, PlayerLink* player)
    {
        OnReload();
        SendReloadEvent(frame, player);
    }

    private void OnReload()
    {
        isReloading = true;
        reloadingTime = reloadTime;
    }

    public void OnReloaded()
    {
        currentAmmo = maxAmmo;
        isReloading = false;
        reloadingTime = FP._0;
    }

    private void Recoil(Frame frame)
    {
        recoilProgressTime +=
            FP._1 / recoilProgressRate * (FP._1 / fireRate);

        var weaponData = frame.FindAsset<WeaponData>(this.data.Id);
        var recoilX = weaponData.HorizontalRecoilCurve.Evaluate(recoilProgressTime);
        var recoilY = weaponData.VerticalRecoilCurve.Evaluate(recoilProgressTime);
    }

    public bool CanFire()
    {
        return IsEndedFireCoolTime() && IsExistAmmo() && !IsReloading() && IsEndedEquipTime();
    }
    
    private bool IsEndedEquipTime()
    {
        return equipTime <= FP._0;
    }

    public bool CanReload()
    {
        return !IsReloading() && !IsAmmoFull();
    }

    private bool IsAmmoFull()
    {
        return currentAmmo >= maxAmmo;
    }

    public bool IsReloading()
    {
        return isReloading;
    }

    private bool IsExistAmmo()
    {
        return currentAmmo > FP._0;
    }

    private bool IsEndedFireCoolTime()
    {
        return nextFireTime <= FP._0;
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