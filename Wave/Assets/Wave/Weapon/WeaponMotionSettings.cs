using System.Collections.Generic;
using KINEMATION.KAnimationCore.Runtime.Attributes;
using KINEMATION.KAnimationCore.Runtime.Rig;
using UnityEngine;

namespace Wave.Weapon
{
    [CreateAssetMenu(fileName = "WeaponMotionSettings", menuName = MenuName, order = 0)]
    public class WeaponMotionSettings : ScriptableObject
    {
        private const string MenuName = "Wave/WeaponMotionSettings";

        [SerializeField] private KRig _rigAsset;
        
        [SerializeField] public KRigElement weaponBone = new(-1, "IK WeaponBone");
        
        public KRig GetRigAsset()
        {
            return _rigAsset;
        }
    }
}