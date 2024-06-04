using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Quantum;
using UnityEngine;

public class WeaponChanger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        QuantumEvent.Subscribe<EventChangeActiveWeapon>(this, ChangeWeapon);
    }

    private void ChangeWeapon(EventChangeActiveWeapon e)
    {
        var type = UnityDB.FindAsset<WeaponDataAsset>(e.NewWeapon.Id).Settings.Type;

        WeaponInventory.Instance.ChangeWeapon(type);
    }
}