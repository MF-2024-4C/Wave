using System.Collections;
using System.Collections.Generic;
using Quantum;
using UnityEngine;

public class WeaponInstancer : MonoBehaviour
{
    [SerializeField] private EntityComponentWeaponInventory _weaponInventory;
    
    public void OnEntityInstantiated()
    {
        QuantumEvent.Subscribe<EventInstanceWeapon>(this, InstanceWeapon);

        InitWeaponInstance();
    }

    private void InitWeaponInstance()
    {
        if (_weaponInventory.Prototype.PrimaryWeaponData != null)
        {
            InstanceWeapon(new EventInstanceWeapon()
            {
                Weapon = _weaponInventory.Prototype.PrimaryWeaponData
            });
        }

        if (_weaponInventory.Prototype.SecondaryWeaponData != null)
        {
            InstanceWeapon(new EventInstanceWeapon()
            {
                Weapon = _weaponInventory.Prototype.SecondaryWeaponData
            });
        }

        if (_weaponInventory.Prototype.TertiaryWeaponData != null)
        {
            InstanceWeapon(new EventInstanceWeapon()
            {
                Weapon = _weaponInventory.Prototype.TertiaryWeaponData
            });
        }
    }

    private void InstanceWeapon(EventInstanceWeapon e)
    {
        var type = UnityDB.FindAsset<WeaponDataAsset>(e.Weapon.Id).Settings.Type;
        var weapon = UnityDB.FindAsset<WeaponDataAsset>(e.Weapon.Id).WeaponPrefab;

        var parent = type switch
        {
            WeaponType.Primary => WeaponInventory.Instance.PrimaryWeaponContainer,
            WeaponType.Secondary => WeaponInventory.Instance.SecondaryWeaponContainer,
            WeaponType.Tertiary => WeaponInventory.Instance.TertiaryWeaponContainer,
            _ => null
        };

        if (parent != null) Instantiate((Object)weapon, parent.transform);
    }
}