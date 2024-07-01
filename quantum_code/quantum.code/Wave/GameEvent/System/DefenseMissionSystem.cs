using Quantum.Events;

namespace Quantum.Wave.GameEvent;

public unsafe class DefenseMissionSystem : SystemMainThreadFilter<DefenseMissionSystem.Filter>
{
    public struct Filter
    {
        public EntityRef Entity;
        public DefenseMission* DefenseMission;
    }
    private GameEventWriter _eventWriter;

    public override void OnInit(Frame f)
    {
        _eventWriter = EventInternal.GetGameEventWriter(f);
    }

    public override void Update(Frame f, ref Filter filter)
    {
        var mission = filter.DefenseMission;
        mission->MissionTimer += f.DeltaTime;
        mission->SpawnEventTimer -= f.DeltaTime;

        if (mission->MissionTimer > mission->MissionEndDelay)
        {
            MissionComplete(f, mission);
            f.Destroy(filter.Entity);
        }

        if (mission->MissionTimer > mission->SpawnEventDelay)
        {
            if (mission->SpawnEventTimer <= 0)
            {
                SpawnEvent(f, mission);
                mission->SpawnEventTimer = mission->SpawnEventInterval;
            }
        }
    }

    private void MissionComplete(Frame f, DefenseMission* mission)
    {
        var writer = EventInternal.GetGameEventWriter(f);
        writer.Write(new MapEvent { ID = mission->MissionEndId }, f);
        Log.Info($"Mission Complete Event ID: {mission->MissionEndId}");
    }

    private void SpawnEvent(Frame f, DefenseMission* mission)
    {
        var writer = EventInternal.GetGameEventWriter(f);
        writer.Write(new MapEvent { ID = mission->SpawnEventId }, f);
        Log.Info($"Spawn Event ID: {mission->SpawnEventId}");
        f.Events.MissionComplete(mission->MissionConfig);
    }
}