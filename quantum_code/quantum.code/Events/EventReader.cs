using System.Runtime.CompilerServices;
using Quantum.Core;

namespace Quantum.Events
{
    public unsafe struct GameEventReader
    {
        public GameEventReader(GameEvents events, FrameBase f)
        {
            _buffer = f.Heap->Void<GameEventsData>(events.container.ptr);
            _eventCounter = _buffer->prevEventCounter;
        }

        private readonly GameEventsData* _buffer;
        private uint _eventCounter;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GameEventsDataIterator Read(FrameBase f)
        {
            var itr = new GameEventsDataIterator(_buffer, _eventCounter, f);
            _eventCounter = _buffer->eventCounter;
            return itr;
        }
    }
}