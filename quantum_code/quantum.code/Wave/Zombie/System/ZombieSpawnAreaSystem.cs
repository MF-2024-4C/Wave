using Quantum.Events;

namespace Quantum;

public unsafe class ZombieSpawnAreaSystem : SystemMainThreadFilter<ZombieSpawnAreaSystem.Filter>
{
    public struct Filter
    {
        public EntityRef Entity;
        public Transform3D* Transform;
        public ZombieSpawnArea* ZombieSpawnArea;
        public EventComponent* EventComponent;
    }
    
    private GameEventReader _eventReader;

    public override void OnInit(Frame f)
    {
        _eventReader = EventInternal.GetGameEventReader(f);
    }

    public override void Update(Frame f, ref Filter filter)
    {
        var spawnArea = filter.ZombieSpawnArea;
        _eventReader = EventInternal.GetGameEventReader(f);

        foreach (var mapEvent in _eventReader.Read(f))
        {
            Log.Info($"Event: {mapEvent.ID}");
            Log.Info($"EventComponent Event: {filter.EventComponent->mapEvent.ID}");
            
            if (mapEvent.ID == filter.EventComponent->mapEvent.ID)
            {
                spawnArea->Active = true;
                Log.Info("SpawnArea Active");
            }
        }
        
        if (!spawnArea->Active)
        {
            return;
        }
        
        var spawnAreaConfig = f.FindAsset<ZombieSpawnAreaConfig>(spawnArea->Config.Id);
        if (spawnArea->CurrentSpawnCount >= spawnArea->MaxSpawnCount)
        {
            return;
        }

        for (var i = 0; i < spawnArea->MaxSpawnCount; i++)
        {
            spawnAreaConfig.Spawn(f, filter.Transform->Position, *spawnArea);
        }

        spawnArea->CurrentSpawnCount = spawnArea->MaxSpawnCount;
    }
}