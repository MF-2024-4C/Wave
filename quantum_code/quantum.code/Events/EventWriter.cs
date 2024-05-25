using System.Runtime.CompilerServices;
using Quantum.Core;

namespace Quantum.Events;

public readonly unsafe struct GameEventWriter
{
    public GameEventWriter(GameEvents events, FrameBase f)
    {
        _buffer = f.Heap->Void<GameEventsData>(events.container.ptr);
    }

    private readonly GameEventsData* _buffer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(in MapEvent value, FrameBase f)
    {
        _buffer->Write(value, f);
    }
}