using System;
using System.Collections;
using System.Collections.Generic;
using Quantum;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponInventory : MonoBehaviour
{
    public static WeaponInventory Instance;

    [SerializeField] private GameObject _primaryWeaponContainer;
    public GameObject PrimaryWeaponContainer => _primaryWeaponContainer;

    [SerializeField] private GameObject _secondaryWeaponContainer;
    public GameObject SecondaryWeaponContainer => _secondaryWeaponContainer;

    [SerializeField] private GameObject _tertiaryWeaponContainer;
    public GameObject TertiaryWeaponContainer => _tertiaryWeaponContainer;

    [HideInInspector] public GunAnimationManager CurrentWeapon;

    private void Awake()
    {
        Instance = this;
    }

    public void OnEntityInstantiated()
    {
        ChangeWeapon(WeaponType.Primary);
    }

    public void ChangeWeapon(WeaponType weaponType)
    {
        GameObject container = null;

        switch (weaponType)
        {
            case WeaponType.Primary:
                container = _primaryWeaponContainer;
                break;
            case WeaponType.Secondary:
                container = _secondaryWeaponContainer;
                break;
            case WeaponType.Tertiary:
                container = _tertiaryWeaponContainer;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var gun = container.GetComponentInChildren<GunAnimationManager>();
        if (gun == null)
        {
            Debug.LogError("GunAnimationManagerがアタッチされていません", container);
            return;
        }

        CurrentWeapon = gun;

        ToggleWeapon(container);
    }

    private void ToggleWeapon(GameObject currentGunContainer)
    {
        _primaryWeaponContainer?.SetActive(false);
        _secondaryWeaponContainer?.SetActive(false);
        _tertiaryWeaponContainer?.SetActive(false);
        currentGunContainer?.SetActive(true);
    }
}