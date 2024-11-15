using System.Runtime.CompilerServices;

namespace Quantum.CrowdNavigation.Utilities;

public static class WorldIndexUtilities
{
    public static Vector3Int IndexToXZY(int index, Vector3Int size)
    {
        int y = index / (size.X * size.Z);
        int z = (index % (size.X * size.Z)) / size.X;
        int x = index % size.X;
        return new Vector3Int(x, y, z);
    }

    public static Vector3Int IndexToXZY(int index, int chunkPowerX, int chunkPowerXZ, int _sizeZ)
    {
        int y = index >> chunkPowerXZ; // y = index / (_sizeX * _sizeZ)
        int z = (index >> chunkPowerX) & (_sizeZ - 1); // z = (index / _sizeX) % _sizeZ
        int x = index & ((1 << chunkPowerX) - 1); // x = index % _sizeX

        return new Vector3Int(x, y, z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int XZYToIndex(int x, int y, int z, int sizeX, int sizeZ)
    {
        return x + (z * sizeX) + (y * sizeX * sizeZ);
    }
 
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WorldXZYToIndex(int x, int y, int z, int chunkSizeLog2X, int chunkSizeLog2Y, int chunkSizeLog2Z,
        Vector3Int dimensionSize)
    {
        Vector3Int chunkPosition = new Vector3Int(
            x >> chunkSizeLog2X,
            y >> chunkSizeLog2Y,
            z >> chunkSizeLog2Z
        );
        int maskX = (1 << chunkSizeLog2X) - 1;
        int maskY = (1 << chunkSizeLog2Y) - 1;
        int maskZ = (1 << chunkSizeLog2Z) - 1;
        Vector3Int localPosition = new Vector3Int(
            x & maskX,
            y & maskY,
            z & maskZ
        );

        int chunkSizeX = 1 << chunkSizeLog2X;
        int chunkSizeY = 1 << chunkSizeLog2Y;
        int localIndex = localPosition.X + chunkSizeX * (localPosition.Y + chunkSizeY * localPosition.Z);

        int chunkIndex = chunkPosition.X + dimensionSize.X * (chunkPosition.Y + dimensionSize.Y * chunkPosition.Z);

        int finalIndex = chunkIndex * (chunkSizeX * chunkSizeY * (1 << chunkSizeLog2Z)) + localIndex;

        return finalIndex;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WorldXZYToChunkLocalIndex(int x, int y, int z, int chunkSizeLog2X, int chunkSizeLog2Y, int chunkSizeLog2Z)
    {
        var maskX = (1 << chunkSizeLog2X) - 1;
        var maskY = (1 << chunkSizeLog2Y) - 1;
        var maskZ = (1 << chunkSizeLog2Z) - 1;
        var localPosition = new Vector3Int(
            x & maskX,
            y & maskY,
            z & maskZ
        );

        var chunkSizeX = 1 << chunkSizeLog2X;
        var chunkSizeY = 1 << chunkSizeLog2Y;
        var localIndex = localPosition.X + chunkSizeX * (localPosition.Y + chunkSizeY * localPosition.Z);

        return localIndex;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WorldXZYToChunkStartIndex(int x, int y, int z, int chunkSizeLog2X, int chunkSizeLog2Y, int chunkSizeLog2Z,
        Vector3Int dimensionSize)
    {
        Vector3Int chunkPosition = new Vector3Int(
            x >> chunkSizeLog2X,
            y >> chunkSizeLog2Y,
            z >> chunkSizeLog2Z
        );

        int chunkSizeX = 1 << chunkSizeLog2X;
        int chunkSizeY = 1 << chunkSizeLog2Y;
        int chunkIndex = chunkPosition.X + dimensionSize.X * (chunkPosition.Y + dimensionSize.Y * chunkPosition.Z);

        int finalIndex = chunkIndex * (chunkSizeX * chunkSizeY * (1 << chunkSizeLog2Z));

        return finalIndex;
    }

    /// <summary>
    /// チャンク座標をセル全体の座標に変換する
    /// </summary>
    /// <param name="chunkPosition">チャンク座標</param>
    /// <param name="chunkSize">1チャンクあたりの大きさ</param>
    /// <returns>チャンクのセル座標</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3Int ChunkPositionToChunkCellPosition(Vector3Int chunkPosition, Vector3Int chunkSize)
    {
        return chunkPosition * chunkSize;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CellPositionToChunkIndex(Vector3Int cellPosition, Vector3Int chunkSize, Vector3Int dimmensionSize)
    {
        return XZYToIndex(cellPosition.X / chunkSize.X, cellPosition.Y / chunkSize.Y, cellPosition.Z / chunkSize.Z,
            dimmensionSize.X, dimmensionSize.Z);
    }

    /// <summary>
    /// Y軸方向にずれるのに必要なインデックス値を返す
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int YJumpFactor(int sizeX, int sizeZ)
    {
        return sizeX * sizeZ;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int XJumpFactor(int sizeX, int sizeZ)
    {
        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ZJumpFactor(int sizeX, int sizeZ)
    {
        return sizeX;
    }

    public static int GetLog2(int value)
    {
        var log = 0;
        while ((value >>= 1) != 0)
        {
            log++;
        }

        return log;
    }

    public static bool IsPowerOfTwo(int num)
    {
        return (num & (num - 1)) == 0;
    }
}