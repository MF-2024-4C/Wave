using System;
using System.Collections;
using System.Collections.Generic;
using Quantum;
using UnityEngine;
using UnityEngine.Serialization;
using Wave.Weapon.Animation;

public class WeaponInventory : MonoBehaviour
{
    [SerializeField] private GameObject _primaryWeaponContainer;
    public GameObject PrimaryWeaponContainer => _primaryWeaponContainer;

    [SerializeField] private GameObject _secondaryWeaponContainer;
    public GameObject SecondaryWeaponContainer => _secondaryWeaponContainer;

    [SerializeField] private GameObject _tertiaryWeaponContainer;
    public GameObject TertiaryWeaponContainer => _tertiaryWeaponContainer;

    [HideInInspector] public WeaponManager CurrentWeapon;
    
    public void ToggleWeapon(GameObject currentGunContainer)
    {
        _primaryWeaponContainer?.SetActive(false);
        _secondaryWeaponContainer?.SetActive(false);
        _tertiaryWeaponContainer?.SetActive(false);
        currentGunContainer?.SetActive(true);
    }
}