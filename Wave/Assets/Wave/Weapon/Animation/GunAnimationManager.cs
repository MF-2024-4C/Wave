using System.Collections;
using System.Collections.Generic;
using Quantum;
using UnityEngine;
using UnityEngine.Serialization;

public unsafe class GunAnimationManager : QuantumCallbacks
{
    [SerializeField] private Animator _animator;

    private const string FireTrigger = "Fire";

    private static readonly int Fire = Animator.StringToHash(FireTrigger);
    

    // Start is called before the first frame update
    void Start()
    {
        if (_animator == null)
        {
            Debug.LogError("アニメーターがアタッチされていません", this);
        }
    }

    public void PlayFireAnimation()
    {
        _animator.SetTrigger(Fire);
    }
    
}