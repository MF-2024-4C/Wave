using System;
using Quantum.Events;

namespace Quantum;

[Serializable]
public abstract class MissionConfig : BaseItemConfig
{
    
}


[Serializable]
public class DefenseMissionTriggerConfig : MissionConfig
{
    public Int32 StartEventID;
    public Int32 EndEventID;


    public Int32 SpawnEventId;
    public int MissionEndDelay;
    public int SpawnEventDelay;
    public int SpawnEventInterval;
    public int MaxSpawnCount;

    public override void Execute(Frame f, EntityRef item, EntityRef player)
    {
        var entity = f.Create();
        f.Add(entity, new DefenseMission()
        {
            MissionConfig = this,
            MissionEndId = EndEventID,
            SpawnEventId = SpawnEventId,
            MissionEndDelay = MissionEndDelay,
            SpawnEventDelay = SpawnEventDelay,
            SpawnEventInterval = SpawnEventInterval,
            MaxSpawnCount = MaxSpawnCount
        });

        var eventWriter = EventInternal.GetGameEventWriter(f);
        eventWriter.Write(new MapEvent { ID = StartEventID }, f);
        Log.Info($"Execute Event ID: {StartEventID}");
        f.Events.MissionStart(this);
    }

    public override void Release(Frame f, EntityRef item, EntityRef player)
    {
    }
}