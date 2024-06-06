using System;
using System.Collections;
using System.Collections.Generic;
using Quantum;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponInventory : MonoBehaviour
{
    [SerializeField] private GameObject _primaryWeaponContainer;
    public GameObject PrimaryWeaponContainer => _primaryWeaponContainer;

    [SerializeField] private GameObject _secondaryWeaponContainer;
    public GameObject SecondaryWeaponContainer => _secondaryWeaponContainer;

    [SerializeField] private GameObject _tertiaryWeaponContainer;
    public GameObject TertiaryWeaponContainer => _tertiaryWeaponContainer;

    [HideInInspector] public GunAnimationManager CurrentWeapon;

    public void OnEntityInstantiated()
    {
        ChangeWeapon(WeaponType.Primary);
    }

    public void ChangeWeapon(WeaponType weaponType)
    {
        var container = weaponType switch
        {
            WeaponType.Primary => _primaryWeaponContainer,
            WeaponType.Secondary => _secondaryWeaponContainer,
            WeaponType.Tertiary => _tertiaryWeaponContainer,
            _ => throw new ArgumentOutOfRangeException()
        };

        var gun = container.GetComponentInChildren<GunAnimationManager>();
        if (gun == null)
        {
            Debug.LogError("GunAnimationManagerがアタッチされていません", container);
            return;
        }

        CurrentWeapon = gun;

        ToggleWeapon(container);
        
        CurrentWeapon.PlayEquipAnimation();
    }

    private void ToggleWeapon(GameObject currentGunContainer)
    {
        _primaryWeaponContainer?.SetActive(false);
        _secondaryWeaponContainer?.SetActive(false);
        _tertiaryWeaponContainer?.SetActive(false);
        currentGunContainer?.SetActive(true);
    }
}