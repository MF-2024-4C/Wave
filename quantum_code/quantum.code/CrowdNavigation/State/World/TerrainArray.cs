using System.Runtime.CompilerServices;
using Photon.Deterministic;
using Quantum.Events;
using Quantum.FF.Collections;

namespace Quantum.FF;

public struct TerrainArray<T> where T : unmanaged
{
    private UnsafeBuffer<T> _data;

    private int _sizeX;
    private int _sizeY;
    private int _sizeZ;
    private int _sizeXZ;
    private int _sizeXZHalf;
    private int _sizeXYZ;
    private int _sizeXYZHalf;

    public TerrainArray(int sizeX, int sizeY, int sizeZ, Native.Allocator allocator)
    {
        if (sizeX <= 0 || sizeY <= 0 || sizeZ <= 0)
        {
            throw new System.ArgumentException("Invalid size.");
        }

        this._sizeX = sizeX;
        this._sizeY = sizeY;
        this._sizeZ = sizeZ;
        _sizeXZ = sizeX * sizeZ;
        _sizeXZHalf = _sizeXZ / 2;
        _sizeXYZ = sizeY * _sizeXZ;
        _sizeXYZHalf = _sizeXYZ / 2;

        if (_sizeXZ % 2 != 0)
        {
            throw new System.ArgumentException("Invalid size.");
        }

        if (_sizeXYZ % 2 != 0)
        {
            throw new System.ArgumentException("Invalid size.");
        }

        _data = new UnsafeBuffer<T>(_sizeXYZ, allocator);
    }

    public void Free(Native.Allocator allocator)
    {
        _data.Free(allocator);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ToIndex(int x, int y, int z)
    {
        return x + z * _sizeX + y * _sizeXZ;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Get(int x, int y, int z)
    {
        return _data[ToIndex(x, y, z)];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(int x, int y, int z, T value)
    {
        _data[ToIndex(x, y, z)] = value;
    }

    public unsafe void CopyFrom(void* src)
    {
        Native.Utils.Copy(_data.GetUnsafePtr(), src, _sizeXYZ * UnsafeUtility.SizeOf<T>());
    }
}