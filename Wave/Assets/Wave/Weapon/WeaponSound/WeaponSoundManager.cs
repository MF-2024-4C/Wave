using UnityEngine;
using UnityEngine.Audio;

public class WeaponSoundManager : MonoBehaviour
{
    [SerializeField] private WeaponManager _weaponManager;
    [SerializeField] private AudioSource _audioSource;
    
    public void PlayWeaponSound(AudioResource audioResource)
    {
        if ( audioResource == null )
        {
            Debug.LogError("AudioResourceがアタッチされていません", this);
            return;
        }
        _audioSource.PlayOneShot(audioResource as AudioClip);
    }
}