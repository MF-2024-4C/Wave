using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quantum.FF;

/// <summary>
/// チャンク間の接続空間を表す構造体
/// </summary>
public struct BoundaryFace
{
    public Vector3Int Min;
    public Vector3Int Max;
    
    public BoundaryFace(Vector3Int min, Vector3Int max)
    {
        Min = min;
        Max = max;
    }
    
    
    /// <summary>
    /// X軸方向の接続空間かどうかを返す
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsLeftOrRight()
    {
        // x = 2,Y = 128,Z = 32 // true
        // x = 32,Y = 2,Z = 32 // false
        // x = 32,Y = 128,Z = 2 // false
        var diff = Max - Min;
        return diff.Z > diff.X && diff.Y > diff.X;
    }
    
    /// <summary>
    /// Y軸方向の接続空間かどうかを返す
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsTopOrBottom()
    {
        // x = 2,Y = 128,Z = 32 // false
        // x = 32,Y = 2,Z = 32 // true
        // x = 32,Y = 128,Z = 2 // false
        var diff = Max - Min;
        return diff.X > diff.Y && diff.Z > diff.Y;
    }
    
    /// <summary>
    /// Z軸方向の接続空間かどうかを返す
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsFrontOrBack()
    {
        // x = 2,Y = 128,Z = 32 // false
        // x = 32,Y = 2,Z = 32 // false
        // x = 32,Y = 128,Z = 2 // true
        var diff = Max - Min;
        return diff.X > diff.Z && diff.Y > diff.Z;
    }
}
public struct WindowNode
{
    public BoundaryFace BoundaryFace;
    public int PortalOffset;
    public int PortalCount;

    public WindowNode(BoundaryFace boundaryFace, int portalOffset, int portalCount)
    {
        BoundaryFace = boundaryFace;
        PortalOffset = portalOffset;
        PortalCount = portalCount;
    }
}