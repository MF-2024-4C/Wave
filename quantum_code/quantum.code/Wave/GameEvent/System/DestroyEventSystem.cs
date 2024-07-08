using Quantum.Events;

namespace Quantum.Wave.GameEvent;

public unsafe class DestroyEventSystem : SystemMainThreadFilter<DestroyEventSystem.Filter>
{
    public struct Filter
    {
        public EntityRef Entity;
        public DestroyEvent* DestroyEvent;
    }
    private GameEventReader _eventReader;

    public override void OnInit(Frame f)
    {
        _eventReader = EventInternal.GetGameEventReader(f);
    }

    public override void Update(Frame f, ref Filter filter)
    {
        foreach (var mapEvent in _eventReader.Read(f))
        {
            if (mapEvent.ID == filter.DestroyEvent->EventId)
            {
                f.Destroy(filter.Entity);
            }
        }
    }
}