using System.Diagnostics;
using Photon.Deterministic;

namespace Quantum;
unsafe partial struct Zombie
{
    public void SetMonsterTarget(Frame f)
    {
        var found = FindTarget(f, out Target);
        Assert.Check(!found,"ゾンビのターゲットが見つかりませんでした。");
    }

    
    private bool FindTarget(Frame f, out EntityRef target)
    {
        target = default;
        var minOrder = FP.MaxValue;
        foreach (var t in f.Unsafe.GetComponentBlockIterator<ZombieTargetable>())
        {
            // 乱数生成
            var rnd1 = f.RNG->Next(FP._0, FP._1);
            var rnd2 = f.RNG->Next(FP._0, FP._1);
            // 正規乱数生成
            var normrand = FPMath.Sqrt(-FP._2 * FPMath.Log2(rnd1)) * FPMath.Cos(FP._2 * FP.Pi * rnd2);
            normrand = normrand * t.Component->sigma + t.Component->order;
            // 最小値更新
            if (normrand >= minOrder) continue;
            minOrder = normrand;
            target = t.Entity;
        }
        
        if (target != default)
        {
            return true;
        }

        return false;
    }
}