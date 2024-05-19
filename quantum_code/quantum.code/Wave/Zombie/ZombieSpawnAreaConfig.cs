using Photon.Deterministic;
using Quantum.Inspector;

namespace Quantum;

public unsafe partial class ZombieSpawnAreaConfig : AssetObject
{
    public AssetRefEntityPrototype Prototype;

    [HideInInspector] public AssetRefZombieSpawnAreaConfig ConfigRef;

    public virtual void Spawn(Frame f, FPVector3 zombieSpawnAreaPosition, FP radius)
    {
        var zombie = f.Create(Prototype);
        var zombieTransform = f.Unsafe.GetPointer<Transform3D>(zombie);
        zombieTransform->Position = GetRandomRadiusPosition(f.RNG, zombieSpawnAreaPosition, radius);
    }

    public FPVector3 GetRandomRadiusPosition(RNGSession* RNG, FPVector3 center, FP radius)
    {
        var radians = RNG->Next(-FP.Pi, FP.Pi);
        var direction = FPVector2.Rotate(FPVector2.Right, radians);
        var rngDistance = RNG->Next(FP._0, radius);
        var position = direction * rngDistance;
        return center + position.XOY;
    }
}