using System.Collections.Generic;

namespace Quantum.Wave.Events;
public unsafe class GameEventSystem : SystemSignalsOnly, ISignalActivateEvent
{
    private readonly Dictionary<Trigger, MapEvent[]> _eventsMap = new();

    public override void OnInit(Frame f)
    {
        
    }
    
    public void ActivateEvent(Frame f, Trigger trigger)
    {
        _eventsMap.TryGetValue(trigger, out var events);
        
        
    }
}