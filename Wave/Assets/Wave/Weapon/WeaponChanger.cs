using System.Collections;
using System.Collections.Generic;
using Quantum;
using UnityEngine;

public class WeaponChanger : MonoBehaviour
{
    [SerializeField] private GameObject _primaryWeapon, _secondaryWeapon, _tertiaryWeapon;

    private EntityView _entityView;

    // Start is called before the first frame update
    void Start()
    {
        //親オブジェクトを取得
        _entityView = GetComponentInParent<EntityView>();

        QuantumEvent.Subscribe<EventChangePrimaryWeapon>(this, ChangePrimaryWeapon);
        QuantumEvent.Subscribe<EventChangeSecondaryWeapon>(this, ChangeSecondaryWeapon);
        QuantumEvent.Subscribe<EventChangeTertiaryWeapon>(this, ChangeTertiaryWeapon);
    }

    private void ChangePrimaryWeapon(EventChangePrimaryWeapon e)
    {
        var frame = QuantumRunner.Default.Game.Frames.Predicted;
        if (e.Player != frame.Get<PlayerLink>(_entityView.EntityRef).Player) return;

        _primaryWeapon.SetActive(true);
        _secondaryWeapon.SetActive(false);
        _tertiaryWeapon.SetActive(false);
    }

    private void ChangeSecondaryWeapon(EventChangeSecondaryWeapon e)
    {
        var frame = QuantumRunner.Default.Game.Frames.Predicted;
        if (e.Player != frame.Get<PlayerLink>(_entityView.EntityRef).Player) return;
        
        _primaryWeapon.SetActive(false);
        _secondaryWeapon.SetActive(true);
        _tertiaryWeapon.SetActive(false);
    }

    private void ChangeTertiaryWeapon(EventChangeTertiaryWeapon e)
    {
        var frame = QuantumRunner.Default.Game.Frames.Predicted;
        if (e.Player != frame.Get<PlayerLink>(_entityView.EntityRef).Player) return;
        
        _primaryWeapon.SetActive(false);
        _secondaryWeapon.SetActive(false);
        _tertiaryWeapon.SetActive(true);
    }
}