using Quantum.Core;
using System.Runtime.CompilerServices;
using Quantum.Wave.Events;

namespace Quantum.Wave.Events
{
    public unsafe struct EventWriter<T>
        where T : unmanaged
    {
        public EventWriter(in Events<T> events)
        {
            buffer = events.GetBuffer();
        }

        EventsData<T>* buffer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in T value, FrameContext context)
        {
            buffer->Write(value, context);
        }
    }
}

namespace Quantum.Wave.Events.Unsafe
{
    public unsafe struct UnsafeEventWriter<T>
        where T : unmanaged
    {
        public UnsafeEventWriter(in UnsafeEvents<T> events)
        {
            buffer = events.buffer;
        }

        EventsData<T>* buffer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in T value, FrameContext context)
        {
            buffer->Write(value, context);
        }
    }
}