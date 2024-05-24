using System.Collections;
using System.Collections.Generic;
using Quantum;
using UnityEngine;

public class FireSystem : MonoBehaviour
{
    [SerializeField] private EntityView _entityView;

    private void Start()
    {
        if (_entityView == null)
        {
            Debug.LogError("EntityViewがアタッチされていません", this);
        }

        QuantumEvent.Subscribe<EventFire>(this, Fire);
    }

    private void Fire(EventFire　e)
    {
        Debug.Log("Fire event received");

        var frame = QuantumRunner.Default.Game.Frames.Predicted;
        if (e.Player != frame.Get<PlayerLink>(_entityView.EntityRef).Player) return;

        WeaponInventory.Instance.UsingGun.PlayFireAnimation();
    }
}