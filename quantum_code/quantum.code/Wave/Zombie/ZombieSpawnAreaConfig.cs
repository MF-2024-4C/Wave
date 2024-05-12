using Photon.Deterministic;
using Quantum.Inspector;

namespace Quantum;

public unsafe partial class ZombieSpawnAreaConfig : AssetObject
{
    public FP StartTime;
    public FP TTL;
    public int SpawnCount;
    public AssetRefEntityPrototype Prototype;

    [HideInInspector] public AssetRefZombieSpawnAreaConfig ConfigRef;
    
    public virtual void Spawn(Frame f, EntityRef zombieSpawnArea)
    {
        var spawnArea = f.Unsafe.GetPointer<ZombieSpawnArea>(zombieSpawnArea);
        var zombie = f.Create(Prototype);

        var transform = f.Unsafe.GetPointer<Transform3D>(zombie);
        
    }
}