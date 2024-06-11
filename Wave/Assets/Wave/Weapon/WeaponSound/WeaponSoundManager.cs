using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class WeaponSoundManager : MonoBehaviour
{
    [SerializeField] private WeaponManager _weaponManager;
    [SerializeField] private AudioSource _audioSource;

    public void PlayFireSound()
    {
        Debug.Log(_weaponManager.WeaponData.FireSound.name);
        _audioSource.PlayOneShot(_weaponManager.WeaponData.FireSound as AudioClip);
    }
}