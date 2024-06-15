using System.Collections;
using System.Collections.Generic;
using Quantum;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponInstancer : MonoBehaviour
{
    [SerializeField] private WeaponInventory _weaponInventory;
    [SerializeField] private EntityComponentWeaponInventory _entityWeaponInventory;
    [SerializeField] private EntityView _entityView;

    public void OnEntityInstantiated()
    {
        QuantumEvent.Subscribe<EventInstanceWeapon>(this, InstanceWeapon);

        InitWeaponInstance();
    }

    private unsafe void InitWeaponInstance()
    {
        var entity = _entityView.EntityRef;
        var frame = QuantumRunner.Default.Game.Frames.Verified.Unsafe;
        var inventory = frame.GetPointer<Quantum.WeaponInventory>(entity);

        var primaryPrototype = inventory->primary;
        if (TryGetWeapon(primaryPrototype, out var primaryWeapon))
        {
            InstanceWeapon(new EventInstanceWeapon()
            {
                Weapon = primaryWeapon->data
            });
        }

        var secondaryPrototype = inventory->secondary;
        if (TryGetWeapon(secondaryPrototype, out var secondaryWeapon))
        {
            InstanceWeapon(new EventInstanceWeapon()
            {
                Weapon = secondaryWeapon->data
            });
        }

        var tertiaryPrototype = inventory->tertiary;
        if (TryGetWeapon(tertiaryPrototype, out var tertiaryWeapon))
        {
            InstanceWeapon(new EventInstanceWeapon()
            {
                Weapon = tertiaryWeapon->data
            });
        }

        return;

        bool TryGetWeapon(EntityRef prototype, out Weapon* weapon)
        {
            weapon = null;
            if (!prototype.IsValid) return false;
            weapon = frame.GetPointer<Weapon>(prototype);
            var result = weapon->data != null;
            weapon = result ? weapon : null;
            return result;
        }
    }

    private void InstanceWeapon(EventInstanceWeapon e)
    {
        var type = UnityDB.FindAsset<WeaponDataAsset>(e.Weapon.Id).Settings.Type;
        var weapon = UnityDB.FindAsset<WeaponDataAsset>(e.Weapon.Id).WeaponPrefab;

        var parent = type switch
        {
            WeaponType.Primary => _weaponInventory.PrimaryWeaponContainer,
            WeaponType.Secondary => _weaponInventory.SecondaryWeaponContainer,
            WeaponType.Tertiary => _weaponInventory.TertiaryWeaponContainer,
            _ => null
        };

        if (parent != null)
        {
            Instantiate(weapon, parent.transform);
        }
    }
}