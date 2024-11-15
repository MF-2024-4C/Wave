using System.Runtime.CompilerServices;

namespace Quantum.FF.Utilities;

public static class Vector3IntExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToIndexXZY(this Vector3Int v, int sizeX, int sizeZ)
    {
        return v.X + v.Z * sizeX + v.Y * sizeX * sizeZ;
    }
    
    public static Vector3Int FromIndexXZY(int index, int sizeX, int sizeZ)
    {
        var y = index / (sizeX * sizeZ);
        var xz = index % (sizeX * sizeZ);
        var z = xz / sizeX;
        var x = xz % sizeX;
        return new Vector3Int(x, y, z);
    }
}