using System.Collections;
using System.Collections.Generic;
using Quantum;
using UnityEngine;
using UnityEngine.Serialization;

public unsafe class GunAnimationManager : QuantumCallbacks
{
    private Animator _animator;

    private const string FireTrigger = "Fire";

    private static readonly int Fire = Animator.StringToHash(FireTrigger);

    private EntityView _entityView;

    // Start is called before the first frame update
    void Start()
    {
        //親オブジェクトを取得
        _entityView = GetComponentInParent<EntityView>();
        
        QuantumEvent.Subscribe<EventFire>(this, DoFire);

        TryGetComponent(out _animator);
    }

    private void DoFire(EventFire　e)
    {
        Debug.Log("Fire event received");

        var frame = QuantumRunner.Default.Game.Frames.Predicted;
        if (e.Player != frame.Get<PlayerLink>(_entityView.EntityRef).Player) return;
        _animator.SetTrigger(Fire);
    }

    // Update is called once per frame
    void Update()
    {
    }
}