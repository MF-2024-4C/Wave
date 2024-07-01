using Quantum.Events;

namespace Quantum.Wave.GameEvent;

public unsafe class DestroyEventSystem : SystemMainThreadFilter<DestroyEventSystem.Filter>
{
    public struct Filter
    {
        public EntityRef Entity;
        public DestroyEvent* DestroyEvent;
    }
    private GameEventWriter _eventWriter;

    public override void OnInit(Frame f)
    {
        _eventWriter = EventInternal.GetGameEventWriter(f);
    }

    public override void Update(Frame f, ref Filter filter)
    {
        var eventReader = EventInternal.GetGameEventReader(f);
        foreach (var mapEvent in eventReader.Read(f))
        {
            if (mapEvent.ID == filter.DestroyEvent->EventId)
            {
                f.Destroy(filter.Entity);
            }
        }
    }
}