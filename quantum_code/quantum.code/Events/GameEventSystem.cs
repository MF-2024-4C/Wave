namespace Quantum.Events;

public unsafe class GameEventSystem : SystemMainThread, ISignalActivateEvent
{
    private GameEventReader _eventReader;
    private GameEventWriter _eventWriter;
    public override void OnInit(Frame f)
    {
        _eventReader = EventInternal.GetGameEventReader(f);
        _eventWriter = EventInternal.GetGameEventWriter(f);
    }

    public void ActivateEvent(Frame f, Trigger trigger)
    {
        _eventWriter.Write(new MapEvent { ID = trigger.ID }, f);
    }

    public override void Update(Frame f)
    {
        var ptr = EventInternal.GetSingletonComponent(f).events.container.ptr;
        f.Heap->Void<GameEventsData>(ptr)->Update(f);

    }
}