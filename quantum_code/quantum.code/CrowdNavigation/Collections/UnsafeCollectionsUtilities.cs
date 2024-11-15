using System;
using Photon.Deterministic;
using Quantum.Events;

namespace Quantum.FF.Collections;

public static class UnsafeCollectionsUtilities
{
    public static unsafe void CopyFrom<T>(this UnsafeBuffer<T> dst, UnsafeSlice<T> src) where T : unmanaged
    {
        if (dst.Length != src.Length)
        {
            throw new ArgumentException("Length mismatch");
        }

        Native.Utils.Copy(dst._buffer, src._buffer, src.Length * UnsafeUtility.SizeOf<T>());
    }
    public static unsafe void CopyFrom<T>(this UnsafeBuffer<T> dst, UnsafeBuffer<T> src) where T : unmanaged
    {
        if (dst.Length != src.Length)
        {
            throw new ArgumentException("Length mismatch");
        }

        Native.Utils.Copy(dst._buffer, src._buffer, src.Length * UnsafeUtility.SizeOf<T>());
    }
}