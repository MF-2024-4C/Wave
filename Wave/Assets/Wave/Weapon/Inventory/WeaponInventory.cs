using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    
    public static WeaponInventory Instance;
    
    [SerializeField] private GunAnimationManager _primaryGun, _secondaryGun;
    [HideInInspector] public GunAnimationManager _usingGun;
    public GunAnimationManager UsingGun => _usingGun;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ChangePrimaryGun();
    }

    public void ChangePrimaryGun()
    {
        _usingGun = _primaryGun;
        _primaryGun.gameObject.SetActive(true);
        _secondaryGun.gameObject.SetActive(false);
    }

    public void ChangeSecondaryGun()
    {
        _usingGun = _secondaryGun;
        _primaryGun.gameObject.SetActive(false);
        _secondaryGun.gameObject.SetActive(true);
    }
    
}