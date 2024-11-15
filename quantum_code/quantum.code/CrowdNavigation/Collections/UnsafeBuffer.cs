using System;
using System.Runtime.CompilerServices;
using Photon.Deterministic;
using Quantum.FF.Utilities;

namespace Quantum.FF.Collections;

public struct UnsafeBuffer<T>
    where T : unmanaged
{
    internal unsafe void* _buffer;
    internal int _length;
    
    public int Length => _length;

    public unsafe UnsafeBuffer(int length, Native.Allocator allocator)
    {
        Allocate(length, allocator, out this);
    }

    public unsafe void Free(Native.Allocator allocator)
    {
        Free(ref this, allocator);
    }

    public unsafe void Clear()
    {
        Native.Utils.Clear(_buffer, _length * sizeof(T));
    }
    
    public unsafe T* GetPointer(int index)
    {
        if (_buffer == null)
            throw new NullReferenceException();
        if ((uint)index >= (uint)_length)
            throw new IndexOutOfRangeException();
        return (T*)((IntPtr)_buffer + (index * sizeof(T)));
    }

    public unsafe ref T this[long index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if ((ulong)index >= (ulong)_length) throw new IndexOutOfRangeException();
            var memoryIndex = index * sizeof(T);
            return ref *(T*)((IntPtr)_buffer + (int)memoryIndex);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void* GetUnsafePtr()
    {
        return _buffer;
    }

    private static unsafe void Allocate(int length, Native.Allocator allocator, out UnsafeBuffer<T> buffer)
    {
        var size = sizeof(T) * length;
        buffer = new UnsafeBuffer<T>
        {
            _buffer = allocator.AllocAndClear(size, UnsafeUtilities.AlignOf<T>()),
            _length = length
        };
    }

    private static unsafe void Free(ref UnsafeBuffer<T> buffer, Native.Allocator allocator)
    {
        if (buffer._buffer == null) return;

        allocator.Free(buffer._buffer);
        buffer._buffer = null;
        buffer._length = 0;
    }
}