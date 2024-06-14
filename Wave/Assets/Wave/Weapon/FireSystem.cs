using System.Collections;
using System.Collections.Generic;
using Quantum;
using UnityEngine;
using Wave;
using Wave.Weapon.Animation;

public class FireSystem : MonoBehaviour
{
    [SerializeField] private EntityView _entityView;
    [SerializeField] private WeaponInventory _weaponInventory;

    private void Start()
    {
        if (_entityView == null)
        {
            Debug.LogError("EntityViewがアタッチされていません", this);
        }

        QuantumEvent.Subscribe<EventFire>(this, Fire);
        QuantumEvent.Subscribe<EventReload>(this, Reload);
        QuantumEvent.Subscribe<EventEquip>(this, Equip);
    }

    private void Fire(EventFire　e)
    {
        if (!this.EqualsPlayer(_entityView, e.Player)) return;

        var anim = WeaponAnimation.Fire;
        _weaponInventory.CurrentWeapon.WeaponAnimationManager.PlayAnimation(anim);

        var frame = QuantumRunner.Default.Game.Frames.Predicted;
        if (frame.TryGet<Weapon>(e.Weapon, out var weapon))
        {
            var sound = UnityDB.FindAsset<WeaponDataAsset>(weapon.data.Id).FireSound;
            _weaponInventory.CurrentWeapon.WeaponSoundManager.PlayWeaponSound(sound);
        }
    }

    private void Reload(EventReload e)
    {
        if (!this.EqualsPlayer(_entityView, e.Player)) return;

        var anim = WeaponAnimation.Reload;
        _weaponInventory.CurrentWeapon.WeaponAnimationManager.PlayAnimation(anim);

        var frame = QuantumRunner.Default.Game.Frames.Predicted;
        if (frame.TryGet<Weapon>(e.Weapon, out var weapon))
        {
            var sound = UnityDB.FindAsset<WeaponDataAsset>(weapon.data.Id).ReloadSound;
            _weaponInventory.CurrentWeapon.WeaponSoundManager.PlayWeaponSound(sound);
        }
    }

    private void Equip(EventEquip e)
    {
        if (!this.EqualsPlayer(_entityView, e.Player)) return;
        
        var anim = WeaponAnimation.Equip;
        _weaponInventory.CurrentWeapon.WeaponAnimationManager.PlayAnimation(anim);

        var frame = QuantumRunner.Default.Game.Frames.Predicted;
        if (frame.TryGet<Weapon>(e.Weapon, out var weapon))
        {
            var sound = UnityDB.FindAsset<WeaponDataAsset>(weapon.data.Id).EquipSound;
            _weaponInventory.CurrentWeapon.WeaponSoundManager.PlayWeaponSound(sound);
        }
    }
}