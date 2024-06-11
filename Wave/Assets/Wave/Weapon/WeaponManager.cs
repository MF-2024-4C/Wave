using System.Collections;
using System.Collections.Generic;
using Quantum;
using UnityEngine;
using UnityEngine.Serialization;
using Wave.Weapon.Animation;

public class WeaponManager : MonoBehaviour
{
    [HideInInspector] public WeaponDataAsset WeaponData;

    [SerializeField] private WeaponAnimationManager _weaponAnimationManager;
    public WeaponAnimationManager WeaponAnimationManager => _weaponAnimationManager;
    [SerializeField] private WeaponSoundManager _weaponSoundManager;
    public WeaponSoundManager WeaponSoundManager => _weaponSoundManager;

    private void Start()
    {
        var weapon = GetComponent<EntityComponentWeapon>();
        WeaponData = UnityDB.FindAsset<WeaponDataAsset>(weapon.Prototype.data.Id);
    }
}