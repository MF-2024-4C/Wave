using System.Runtime.CompilerServices;
using Quantum.Wave.Events;

namespace Quantum.Wave.Events
{
    public unsafe struct EventReader<T>
        where T : unmanaged
    {
        public EventReader(in Events<T> events)
        {
            buffer = events.GetBuffer();
            eventCounter = buffer->prevEventCounter;
        }

        EventsData<T>* buffer;
        uint eventCounter;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EventsDataIterator<T> Read()
        {
            var itr = new EventsDataIterator<T>(buffer, eventCounter);
            eventCounter = buffer->eventCounter;
            return itr;
        }
    }
}

namespace Quantum.Wave.Events.Unsafe
{
    public unsafe struct UnsafeEventReader<T>
        where T : unmanaged
    {
        public UnsafeEventReader(in UnsafeEvents<T> events)
        {
            buffer = events.buffer;
            eventCounter = buffer->prevEventCounter;
        }

        EventsData<T>* buffer;
        uint eventCounter;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EventsDataIterator<T> Read()
        {
            var itr = new EventsDataIterator<T>(buffer, eventCounter);
            eventCounter = buffer->eventCounter;
            return itr;
        }
    }
}