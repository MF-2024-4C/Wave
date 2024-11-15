using System;
using System.Runtime.CompilerServices;

namespace Quantum.FF.Collections;

public struct UnsafeSlice<T> where T : unmanaged
{
    internal unsafe byte* _buffer;
    internal int _stride;
    internal int _length;
    
    public int Length => _length;
    public int Size => _length * _stride;

    public unsafe UnsafeSlice(UnsafeBuffer<T> buffer, int start, int length)
    {
        _stride = sizeof(T);
        _length = length;
        _buffer = (byte*)((IntPtr)buffer.GetUnsafePtr() + this._stride * start);
    }

    public unsafe T* GetPointer(int index)
    {
        if (_buffer == null)
            throw new NullReferenceException();
        if ((uint)index >= (uint)_length)
            throw new IndexOutOfRangeException();
        return (T*)((IntPtr)_buffer + (index * sizeof(T)));
    }
    
    public unsafe void* GetUnsafePtr()
    {
        return _buffer;
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
}