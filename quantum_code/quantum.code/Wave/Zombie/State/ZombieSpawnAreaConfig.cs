using Photon.Deterministic;
using Quantum.Inspector;

namespace Quantum;

public unsafe partial class ZombieSpawnAreaConfig : AssetObject
{
    public AssetRefEntityPrototype Prototype;

    [HideInInspector] public AssetRefZombieSpawnAreaConfig ConfigRef;

    public virtual void Spawn(Frame f, FPVector3 zombieSpawnAreaPosition,in ZombieSpawnArea spawnArea)
    {
        var zombieEntityRef = f.Create(Prototype);
        var zombieTransform = f.Unsafe.GetPointer<Transform3D>(zombieEntityRef);
        var zombie = f.Unsafe.GetPointer<Zombie>(zombieEntityRef);
        
        zombieTransform->Position = GetRandomRadiusPosition(f.RNG, zombieSpawnAreaPosition, spawnArea.Radius);
        
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