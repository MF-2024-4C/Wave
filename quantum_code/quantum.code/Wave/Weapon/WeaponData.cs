using Photon.Deterministic;

namespace Quantum;

public partial class WeaponData
{
    public WeaponType Type;

    public FireMode FireMode;
    public bool IsFullAuto;
    public bool IsSemiAuto;

    public FP FireRate;
    public FP ReloadTime;
    public int maxAmmo;
    public int damage;

    /// <summary>
    /// WeaponDataを追加した場合、CopyDataメソッドに追加すること。
    /// </summary>
    /// <param name="weapon"></param>
    public unsafe void CopyData(Weapon* weapon)
    {
        weapon->type = Type;

        weapon->currentFireMode = FireMode;
        weapon->isFullAuto = IsFullAuto;
        weapon->isSemiAuto = IsSemiAuto;

        weapon->fireRate = FireRate;
        weapon->reloadTime = ReloadTime;
        weapon->maxAmmo = maxAmmo;
        weapon->damage = damage;
    }

    public unsafe void Initialize(Weapon* weapon)
    {
        CopyData(weapon);
        weapon->currentAmmo = maxAmmo;
    }
}