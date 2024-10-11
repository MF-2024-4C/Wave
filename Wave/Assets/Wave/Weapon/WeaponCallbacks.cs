using Quantum;
using UnityEngine;

namespace Wave.Weapon
{
    public abstract class WeaponCallbacks : MonoBehaviour
    {
        protected virtual void OnFire(EventFire e){}
        protected virtual void OnAttackHitLocal(EventOnPlayerAttackHitLocal e){}
        protected virtual void OnReloaded(EventReload e){}
        protected virtual void OnReloadCompleted(EventReloadComplete e){}
        
        protected virtual void OnWeaponChanged(EventChangeActiveWeapon e){}
        protected virtual void OnWeaponEquip(EventEquip e){}
        protected virtual void OnInventoryUpdated(EventInventoryUpdate e){}
    }
}