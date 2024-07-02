using Photon.Deterministic;
using Quantum.Inspector;

namespace Quantum;

public unsafe partial class ZombieSpec : AssetObject
{
    [Tooltip("スポーン時の体力")]
    public FP HP;
    [Tooltip("休止時の索敵距離")]
    public FP DormantSearchDistance;
    [Tooltip("攻撃範囲")]
    public FP AttackRange;
}