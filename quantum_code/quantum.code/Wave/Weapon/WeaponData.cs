using Photon.Deterministic;

namespace Quantum;

public enum WeaponType
{
    Primary,
    Secondary,
    Tertiary
}

public partial class WeaponData
{
    public WeaponType Type;
    public FP FireRate;
    public FP ReloadTime;
}