using UnityEngine;
using Wave.Weapon;
using Wave.Weapon.Animation;

public class WeaponManager : MonoBehaviour
{
    [HideInInspector] public WeaponDataAsset WeaponData;

    [SerializeField] private WeaponAnimationManager _weaponAnimationManager;
    public WeaponAnimationManager WeaponAnimationManager => _weaponAnimationManager;
    [SerializeField] private WeaponSoundManager _weaponSoundManager;
    public WeaponSoundManager WeaponSoundManager => _weaponSoundManager;

    public WeaponView WeaponView { get; private set; }

    private void Awake()
    {
        var weapon = GetComponent<EntityComponentWeapon>();
        WeaponData = UnityDB.FindAsset<WeaponDataAsset>(weapon.Prototype.data.Id);
        WeaponView = GetComponent<WeaponView>();
        WeaponView.Initialize(WeaponData.Settings.FireMode.ToKinemationFireMode(), WeaponData.Settings.FireRate.AsFloat * 60f);
    }
}