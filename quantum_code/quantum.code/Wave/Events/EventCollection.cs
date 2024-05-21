using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Photon.Deterministic;
using Quantum.Core;

namespace Quantum.Wave.Events;
public struct EventCollection<T> where T : unmanaged
{
    private unsafe T* _ptr;
    private int _count;

    public int Count
    {
        get => this._count;
        internal set
        {
            Quantum.Assert.Check((long) (uint) value <= (long) this._capacity);
            this._count = value;
        }
    }


    private int _capacity;

    internal unsafe EventCollection(int capacity, Native.Allocator allocator)
    {
        Quantum.Assert.Check(capacity > 0);
        this._count = 0;
        this._capacity = capacity;
        this._ptr = (T*) allocator.AllocAndClear(this._capacity * sizeof(T));
    }

    internal unsafe void Clear(Native.Allocator allocator)
    {
        if ((IntPtr) this._ptr != IntPtr.Zero)
        {
            allocator.Free((void*) this._ptr);
            this._ptr = (T*) null;
        }

        this._capacity = 0;
        this._count = 0;
    }

    internal unsafe void Add(T hit, FrameContext context)
    {
        if (this._count == this._capacity)
            this.ExpandCapacity(this._capacity * 2, context);
        this._ptr[this._count++] = hit;
    }

    private unsafe void ExpandCapacity(int newCapacity, FrameContext context)
    {
        Quantum.Assert.Check(newCapacity > this._capacity);
        var dest = (T*) context.TempAllocateAndClear(newCapacity * sizeof(T));
        Native.Utils.Copy((void*) dest, (void*) this._ptr, sizeof(T) * this._capacity);
        this._ptr = dest;
        this._capacity = newCapacity;
    }

    public unsafe ParallelReader AsParallelReader()
    {
        return new ParallelReader(_ptr, Count);
    }

    public unsafe struct ParallelReader
    {
        public readonly T* Ptr;

        public readonly int Length;

        internal ParallelReader(T* ptr, int length)
        {
            Ptr = ptr;
            Length = length;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ParallelWriter AsParallelWriter()
    {
        fixed (EventCollection<T>* ptr = &this)
        {
            return new ParallelWriter(ptr);
        }
    }

    public unsafe struct ParallelWriter
    {
        public readonly void* Ptr => ListData->_ptr;

        public EventCollection<T>* ListData;

        internal unsafe ParallelWriter(EventCollection<T>* listData)
        {
            ListData = listData;
        }
    }
}