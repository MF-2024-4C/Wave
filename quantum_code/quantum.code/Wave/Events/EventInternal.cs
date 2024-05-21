using System;
using System.Runtime.CompilerServices;
using Photon.Deterministic;
using Quantum.Core;
using Quantum.Wave.Events.Unsafe;

namespace Quantum.Wave.Events;
public static unsafe class UnsafeUtility
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SizeOf<T>() where T : unmanaged => sizeof(T);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void CopyStructureToPtr<T>(ref T input, void* ptr) where T : unmanaged
    {
        if ((IntPtr) ptr == IntPtr.Zero)
            throw new ArgumentNullException();
        InternalCopyStructureToPtr(ref input, ptr);
    }

    private static unsafe void InternalCopyStructureToPtr<T>(ref T input, void* ptr) where T : unmanaged
    {
        *(T*) ptr = input;
    }
}

public unsafe struct UnsafeEvents<T>
    where T : unmanaged
{
    public UnsafeEvents(int initialCapacity, Native.Allocator allocator)
    {
        buffer = this.allocator.AllocAndClear<EventsData<T>>();
        var data = new EventsData<T>(initialCapacity, allocator);
        UnsafeUtility.CopyStructureToPtr(ref data, buffer);
        this.allocator = allocator;
    }

    internal EventsData<T>* buffer;
    readonly Native.Allocator allocator;

    public bool IsCreated
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { return buffer != null; }
    }

    public void Free(FrameContext frameContext)
    {
        buffer->Free(frameContext);
        allocator.Free((void*) this.buffer);
        this.buffer = null;
        buffer = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Update(Native.Allocator allocator)
    {
        buffer->Update(allocator);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UnsafeEventWriter<T> GetWriter()
    {
        return new UnsafeEventWriter<T>(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UnsafeEventReader<T> GetReader()
    {
        return new UnsafeEventReader<T>(this);
    }
}

public struct Events<T>
    where T : unmanaged
{
    public Events(int initialCapacity, Native.Allocator allocator)
    {
        _container = new UnsafeEvents<T>(initialCapacity, allocator);
    }

    public void Free(FrameContext frameContext)
    {
        _container.Free(frameContext);
    }

    private UnsafeEvents<T> _container;
    internal readonly unsafe EventsData<T>* GetBuffer() => _container.buffer;
}

public static unsafe class EventInternal
{
    public static EventReader<T> GetEventReader<T>(Frame frame)
        where T : unmanaged, IComponentSingleton
    {
        //return frame.Unsafe.GetOrAddSingletonPointer<T>();
        return new EventReader<T>();
    }
    
}