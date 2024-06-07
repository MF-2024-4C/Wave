using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Quantum;
using UnityEngine;

public class WeaponChanger : MonoBehaviour
{
    [SerializeField] private EntityView _entityView;
    [SerializeField] private WeaponInventory _weaponInventory;

    // Start is called before the first frame update
    void Start()
    {
        QuantumEvent.Subscribe<EventChangeActiveWeapon>(this, ChangeWeapon);
    }

    private void ChangeWeapon(EventChangeActiveWeapon e)
    {
        var frame = QuantumRunner.Default.Game.Frames.Predicted;
        if (e.Player != frame.Get<PlayerLink>(_entityView.EntityRef).Player) return;

        var type = UnityDB.FindAsset<WeaponDataAsset>(e.NewWeapon.Id).Settings.Type;

        _weaponInventory.ChangeWeapon(type);

    
    }
}