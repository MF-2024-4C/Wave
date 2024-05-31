using System.Collections;
using System.Collections.Generic;
using Quantum;
using UnityEngine;
using UnityEngine.Serialization;

public unsafe class GunAnimationManager : QuantumCallbacks
{
    [SerializeField] private Animator _animator;

    private static readonly int Fire = Animator.StringToHash("Fire");
    private static readonly int Reload = Animator.StringToHash("Reload");

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
    
    public void PlayReloadAnimation()
    {
        _animator.SetTrigger(Reload);
    }
}