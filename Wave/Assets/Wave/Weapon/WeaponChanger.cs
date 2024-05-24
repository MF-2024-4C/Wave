using System.Collections;
using System.Collections.Generic;
using Quantum;
using UnityEngine;

public class WeaponChanger : MonoBehaviour
{
    [SerializeField] private EntityView _entityView;

    // Start is called before the first frame update
    void Start()
    {
        if (_entityView == null)
        {
            Debug.LogError("EntityViewがアタッチされていません", this);
        }

        QuantumEvent.Subscribe<EventChangePrimaryWeapon>(this, ChangePrimaryWeapon);
        QuantumEvent.Subscribe<EventChangeSecondaryWeapon>(this, ChangeSecondaryWeapon);
        QuantumEvent.Subscribe<EventChangeTertiaryWeapon>(this, ChangeTertiaryWeapon);
    }

    private void ChangePrimaryWeapon(EventChangePrimaryWeapon e)
    {
        var frame = QuantumRunner.Default.Game.Frames.Predicted;
        if (e.Player != frame.Get<PlayerLink>(_entityView.EntityRef).Player) return;

        WeaponInventory.Instance.ChangePrimaryGun();
    }

    private void ChangeSecondaryWeapon(EventChangeSecondaryWeapon e)
    {
        var frame = QuantumRunner.Default.Game.Frames.Predicted;
        if (e.Player != frame.Get<PlayerLink>(_entityView.EntityRef).Player) return;

        WeaponInventory.Instance.ChangeSecondaryGun();
    }

    private void ChangeTertiaryWeapon(EventChangeTertiaryWeapon e)
    {
        var frame = QuantumRunner.Default.Game.Frames.Predicted;
        if (e.Player != frame.Get<PlayerLink>(_entityView.EntityRef).Player) return;

        //WeaponInventory.Instance.ChangeTertiaryGun();
    }
}