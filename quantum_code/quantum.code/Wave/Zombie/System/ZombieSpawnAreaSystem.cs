using System;
using Photon.Deterministic;
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

    private const short SpawnPerFrame = 10;
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
                if (spawnArea->Active)
                {
                    Log.Warn($"Event ID: [{mapEvent.ID}] SpawnArea is already Active.");
                }else
                {
                    spawnArea->Active = true;
                    spawnArea->CurrentSpawnCount = 0;
                    Log.Info("SpawnArea Active");
                }
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

        var spawnCount = (short)Math.Min(spawnArea->CurrentSpawnCount + SpawnPerFrame, spawnArea->MaxSpawnCount);
        for (var i = spawnArea->CurrentSpawnCount; i < spawnCount; i++)
        {
            spawnAreaConfig.Spawn(f, filter.Transform->Position, *spawnArea);
        }
        spawnArea->CurrentSpawnCount += spawnCount;
        
        if(spawnArea->CurrentSpawnCount >= spawnArea->MaxSpawnCount)
        {
            spawnArea->Active = false;
            
        }
    }
}