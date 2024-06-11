using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Quantum;
using UnityEngine;
using Wave.Weapon.Animation;

public class WeaponChanger : MonoBehaviour
{
    [SerializeField] private EntityView _entityView;
    [SerializeField] private WeaponInventory _weaponInventory;

    // Start is called before the first frame update
    void Start()
    {
        QuantumEvent.Subscribe<EventChangeActiveWeapon>(this, ChangeWeapon);
    }

    public void OnEntityInstantiated()
    {
        ChangeWeaponFromType(WeaponType.Primary);
    }

    private void ChangeWeaponFromType(WeaponType type)
    {
        var container = GetContainerFromType(type);

        var gun = container.GetComponentInChildren<WeaponManager>();
        if (gun == null)
        {
            Debug.LogError("WeaponManagerがアタッチされていません", container);
            return;
        }

        _weaponInventory.CurrentWeapon = gun;

        _weaponInventory.ToggleWeapon(container);

        _weaponInventory.CurrentWeapon.WeaponAnimationManager.PlayAnimation(WeaponAnimation.Equip);
    }

    private void ChangeWeapon(EventChangeActiveWeapon e)
    {
        var frame = QuantumRunner.Default.Game.Frames.Predicted;
        if (e.Player != frame.Get<PlayerLink>(_entityView.EntityRef).Player) return;

        var type = UnityDB.FindAsset<WeaponDataAsset>(e.NewWeapon.Id).Settings.Type;

        ChangeWeaponFromType(type);
    }
    
    private GameObject GetContainerFromType(WeaponType type)
    {
        var container = type switch
        {
            WeaponType.Primary => _weaponInventory.PrimaryWeaponContainer,
            WeaponType.Secondary => _weaponInventory.SecondaryWeaponContainer,
            WeaponType.Tertiary => _weaponInventory.TertiaryWeaponContainer,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        return container;
    }
}