using Photon.Deterministic;
using Quantum.Inspector;

namespace Quantum;

public partial class WeaponData
{
    public WeaponType Type;

    [Tooltip("初期の射撃モード")] public FireMode FireMode;
    [Tooltip("フルオート射撃（複数選択可")] public bool IsFullAuto;
    [Tooltip("セミオート射撃（複数選択可")] public bool IsSemiAuto;

    [Tooltip("1秒間に発射できる弾数")] public FP FireRate;
    [Tooltip("リロードするのにかかる時間")] public FP ReloadTime;
    [Tooltip("マガジンに入る弾の弾数")] public int MaxAmmo;
    [Tooltip("1発のダメージ")] public int Damage;
    [Tooltip("弾の貫通力")] public int PenetrationPower;
    [Tooltip("武器を取り出す時間")] public FP EquipTime;

    [Tooltip("射撃時の横反動")] public FPAnimationCurve HorizontalRecoilCurve;
    [Tooltip("射撃時の縦反動")] public FPAnimationCurve VerticalRecoilCurve;

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
        weapon->maxAmmo = MaxAmmo;
        weapon->damage = Damage;
        weapon->penetrationPower = PenetrationPower;

        weapon->equipTime = EquipTime;
    }

    public unsafe void Initialize(Weapon* weapon)
    {
        CopyData(weapon);
        weapon->currentAmmo = MaxAmmo;
        weapon->recoilProgressRate = FP._1 / FireRate * MaxAmmo;
    }

    public unsafe void OnEquip(Frame frame, PlayerLink* playerLink, Weapon* weapon, EntityRef weaponEntity)
    {
        weapon->equipTime = EquipTime;

        frame.Events.Equip(playerLink->Player, weaponEntity);
    }

    public unsafe void OnUnEquip(Weapon* weapon)
    {
        weapon->nextFireTime = FP._0;

        weapon->reloadingTime = FP._0;
        weapon->isReloading = false;

        weapon->recoilProgressTime = FP._0;
    }
}