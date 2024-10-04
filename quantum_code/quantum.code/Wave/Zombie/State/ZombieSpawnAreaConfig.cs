using Photon.Deterministic;
using Quantum.Inspector;

namespace Quantum;

public unsafe partial class ZombieSpawnAreaConfig : AssetObject
{
    public AssetRefEntityPrototype Prototype;

    [HideInInspector] public AssetRefZombieSpawnAreaConfig ConfigRef;

    public virtual void Spawn(Frame f, FPVector3 zombieSpawnAreaPosition, in ZombieSpawnArea spawnArea)
    {
        var zombieEntityRef = f.Create(Prototype);
        var zombieTransform = f.Unsafe.GetPointer<Transform3D>(zombieEntityRef);
        var zombie = f.Unsafe.GetPointer<Zombie>(zombieEntityRef);
        var agent = f.Unsafe.GetPointer<NavMeshSteeringAgent>(zombieEntityRef);
        agent->MaxSpeed = f.RNG->Next(FP._2, FP._4);
        if (spawnArea.isHorde)
        {
            var targetCount = 0;
            foreach (var t in f.Unsafe.GetComponentBlockIterator<ZombieTargetable>())
            {
                targetCount++;
            }

            var index = 0;
            foreach (var t in f.Unsafe.GetComponentBlockIterator<ZombieTargetable>())
            {
                f.Unsafe.TryGetPointer<Transform3D>(t.Entity, out var transform);

                if (zombieEntityRef.Index % targetCount == index)
                {
                    zombie->Target = t.Entity;
                    zombie->State = ZombieState.Chase;
                    break;
                }

                index++;
            }
        }

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