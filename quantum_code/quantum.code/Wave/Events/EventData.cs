using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum.Core;

namespace Quantum.Wave.Events;
public readonly struct EventInstance<T>  where T : unmanaged
{
    public readonly T value;
    public readonly uint id;

    public EventInstance(in T value, uint id)
    {
        this.value = value;
        this.id = id;
    }
}

public struct EventsData<T>
    where T : unmanaged
{
    internal EventCollection<EventInstance<T>> buffer1;
    internal EventCollection<EventInstance<T>> buffer2;
    internal uint eventCounter;
    internal uint prevEventCounter;
    bool state;

    internal EventCollection<EventInstance<T>> GetWriteBuffer() => state ? buffer2 : buffer1;
    internal EventCollection<EventInstance<T>> GetReadBuffer() => state ? buffer1 : buffer2;

    public EventsData(int capacity, Native.Allocator allocator)
    {
        buffer1 = new EventCollection<EventInstance<T>>(capacity, allocator);
        buffer2 = new EventCollection<EventInstance<T>>(capacity, allocator);
        eventCounter = 0;
        prevEventCounter = 0;
        state = false;
    }

    public void Update(Native.Allocator allocator)
    {
        state = !state;
        if (state) buffer2.Clear(allocator);
        else buffer1.Clear(allocator);

        prevEventCounter = eventCounter;
    }

    public void Write(in T value, FrameContext context)
    {
        if (state) buffer2.Add(new EventInstance<T>(value, eventCounter), context);
        else buffer1.Add(new EventInstance<T>(value, eventCounter), context);
        eventCounter++;
    }

    public void Free(FrameContext context)
    {
        buffer1.Clear(context.Allocator);
        buffer2.Clear(context.Allocator);
    }
}

public readonly unsafe ref struct EventsDataIterator<T> where T : unmanaged
{
    public EventsDataIterator(EventsData<T>* buffer, uint eventCounter)
    {
        this.buffer = buffer;
        this.eventCounter = eventCounter;
    }

    readonly EventsData<T>* buffer;
    readonly uint eventCounter;

    public Enumerator GetEnumerator()
    {
        return new Enumerator(buffer->GetReadBuffer(), buffer->GetWriteBuffer(), eventCounter);
    }

    public struct Enumerator : IEnumerator<T>
    {
        public Enumerator(EventCollection<EventInstance<T>> buffer1, EventCollection<EventInstance<T>> buffer2, uint eventCounter)
        {
            reader1 = buffer1.AsParallelReader();
            reader2 = buffer2.AsParallelReader();
            this.eventCounter = eventCounter;
            current = default;
            offset = default;
            readFirstReader = default;
        }

        readonly EventCollection<EventInstance<T>>.ParallelReader reader1;
        readonly EventCollection<EventInstance<T>>.ParallelReader reader2;
        readonly uint eventCounter;
        T current;
        int offset;
        bool readFirstReader;

        public T Current => current;
        object IEnumerator.Current => current;

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            var reader = readFirstReader ? reader2 : reader1;
            if (reader.Ptr != null && reader.Length > offset)
            {
                ref var instance = ref *(reader.Ptr + offset);
                offset++;

                if (instance.id < eventCounter) return MoveNext();
                current = instance.value;
                return true;
            }
            else if (!readFirstReader)
            {
                readFirstReader = true;
                offset = 0;
                return MoveNext();
            }

            return false;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }
    }
}