using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Photon.Deterministic;
using Quantum.CrowdNavigation.Utilities;
using Quantum.FF.Collections;

namespace Quantum.FF;

[Serializable]
public struct GroundHeightMapManaged
{
    public byte[] Data;

    public void Initialize(int sizeX, int sizeY, int sizeZ)
    {
        Data = new byte[sizeX * sizeY * sizeZ];
    }

    public void Write(int index, int y, int sizeX, int sizeZ)
    {
        var bottomJumpFactor = WorldIndexUtilities.YJumpFactor(sizeX, sizeZ);
        while (Data.Length > index)
        {
            Data[index] = (byte)y;
            index += bottomJumpFactor;
        }
    }

    public byte Read(int index)
    {
        return Data[index];
    }
}

public ref struct ReadOnlyGroundHeightMap
{
    public ReadOnlySpan<byte> Data;

    public ReadOnlyGroundHeightMap(ReadOnlySpan<byte> data)
    {
        Data = data;
    }

    public readonly byte Read(int x, int y, int z, int sizeX, int sizeZ)
    {
        var index = WorldIndexUtilities.XZYToIndex(x, y, z, sizeX, sizeZ);
        return Data[index];
    }

    public readonly byte Read(int index)
    {
        return Data[index];
    }
}

public struct GroundHeightMap
{
    private UnsafeBuffer<ushort> _data;
    private int _sizeX;
    private int _sizeY;
    private int _sizeZ;
    private int _sizeXZ;
    private int _sizeXZHalf;
    private int _sizeXYZ;
    private int _sizeXYZHalf;

    public GroundHeightMap(int sizeX, int sizeY, int sizeZ, Native.Allocator allocator)
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

        _data = new UnsafeBuffer<ushort>(_sizeXYZ, allocator);
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
    public ushort Get(int x, int y, int z)
    {
        return _data[ToIndex(x, y, z)];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(int x, int y, int z, ushort value)
    {
        _data[ToIndex(x, y, z)] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(int index, ushort value)
    {
        _data[index] = value;
    }
}