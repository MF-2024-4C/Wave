using System;
using System.Runtime.CompilerServices;

namespace Quantum.Events;

public static unsafe class UnsafeUtility
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SizeOf<T>() where T : unmanaged => sizeof(T);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyStructureToPtr<T>(ref T input, void* ptr) where T : unmanaged
    {
        if ((IntPtr)ptr == IntPtr.Zero)
            throw new ArgumentNullException();
        InternalCopyStructureToPtr(ref input, ptr);
    }

    private static void InternalCopyStructureToPtr<T>(ref T input, void* ptr) where T : unmanaged
    {
        *(T*)ptr = input;
    }
}


public static class EventInternal
{
    public static GameEvent GetSingletonComponent(Frame f)
    {
        if (f.TryGetSingleton<GameEvent>(out var singleton)) return singleton;
        var events = new GameEvents(512, f);
        singleton = new GameEvent { events = events };
        f.SetSingleton(singleton);
        return singleton;
    }

    public static GameEventWriter GetGameEventWriter(Frame f)
    {
        return GetSingletonComponent(f).events.GetWriter(f);
    }

    public static GameEventReader GetGameEventReader(Frame f)
    {
        return GetSingletonComponent(f).events.GetReader(f);
    }
}