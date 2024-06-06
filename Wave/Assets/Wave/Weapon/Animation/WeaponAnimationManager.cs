using UnityEngine;

namespace Wave.Weapon.Animation
{
    /// <summary>
    /// 武器のアニメーションの種類
    /// AnimatorControllerのステートと同じ名前にしてください
    /// </summary>
    public enum WeaponAnimation
    {
        Fire,
        Reload,
        Equip,
        UnEquip
    }

    public unsafe class WeaponAnimationManager : QuantumCallbacks
    {
        [SerializeField] private Animator _animator;
        
        // Start is called before the first frame update
        void Start()
        {
            if (_animator == null)
            {
                Debug.LogError("アニメーターがアタッチされていません", this);
            }
        }

        public void PlayAnimation(WeaponAnimation type)
        {
            var animationName = type.ToString();
            _animator.CrossFadeInFixedTime(animationName, 0);
        }
    }
}