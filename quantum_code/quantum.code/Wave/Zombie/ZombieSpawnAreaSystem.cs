namespace Quantum;

public unsafe class ZombieSpawnAreaSystem : SystemMainThreadFilter<ZombieSpawnAreaSystem.Filter>
{
    public struct Filter
    {
        public EntityRef Entity;
        public Transform3D* Transform;
        public ZombieSpawnArea* ZombieSpawnArea;
    }

    public override void Update(Frame f, ref Filter filter)
    {
        var spawnArea = filter.ZombieSpawnArea;

        var spawnAreaConfig = f.FindAsset<ZombieSpawnAreaConfig>(spawnArea->Config.Id);
        if (spawnArea->CurrentSpawnCount >= spawnArea->MaxSpawnCount)
        {
            return;
        }

        for (var i = 0; i < spawnArea->MaxSpawnCount; i++)
        {
            spawnAreaConfig.Spawn(f, filter.Transform->Position, spawnArea->Radius);
        }

        spawnArea->CurrentSpawnCount = spawnArea->MaxSpawnCount;
    }
}