using KINEMATION.FPSAnimationFramework.Runtime.Recoil;

namespace Wave.Weapon
{
    public static class WeaponViewAdapter
    {
        public static FireMode ToKinemationFireMode(this Quantum.FireMode quantumFireMode)
        {
            switch (quantumFireMode)
            {
                case Quantum.FireMode.SemiAuto:
                    return FireMode.Semi;
                case Quantum.FireMode.FullAuto:
                    return FireMode.Auto;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(quantumFireMode), quantumFireMode, null);
            }
        }
    }
}