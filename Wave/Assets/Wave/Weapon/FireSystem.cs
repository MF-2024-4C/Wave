using System.Collections;
using System.Collections.Generic;
using Quantum;
using UnityEngine;
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
    }

    private void Fire(EventFire　e)
    {
        var frame = QuantumRunner.Default.Game.Frames.Predicted;
        if (e.Player != frame.Get<PlayerLink>(_entityView.EntityRef).Player) return;

        _weaponInventory.CurrentWeapon.PlayAnimation(WeaponAnimation.Fire);
    }

    private void Reload(EventReload e)
    {
        var frame = QuantumRunner.Default.Game.Frames.Predicted;
        if (e.Player != frame.Get<PlayerLink>(_entityView.EntityRef).Player) return;

        _weaponInventory.CurrentWeapon.PlayAnimation(WeaponAnimation.Reload);
    }
}