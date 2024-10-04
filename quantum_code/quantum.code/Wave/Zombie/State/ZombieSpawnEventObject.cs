using System;
using Photon.Deterministic;
using Quantum.Events;

namespace Quantum;

[Serializable]
public unsafe partial class ZombieSpawnItemConfig : BaseItemConfig
{
    public Int32 EventID;

    public override void Execute(Frame f, EntityRef item, EntityRef player)
    {
        var eventWriter = EventInternal.GetGameEventWriter(f);
        eventWriter.Write(new MapEvent { ID = EventID }, f);
        Log.Info($"Execute Event ID: {EventID}");
    }

    public override void Release(Frame f, EntityRef item, EntityRef player)
    {
    }
}