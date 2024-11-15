using System;
using System.Runtime.CompilerServices;
using Quantum.CrowdNavigation.Utilities;
using Quantum.FF.Collections;

namespace Quantum.FF;

public ref struct FieldGraphBuilder
{
    //書き込みフィールド
    public UnsafeBuffer<SectorNode> SectorNodes;
    public UnsafeBuffer<int> SectorToWindowOffsets;
    public UnsafeBuffer<WindowNode> WindowNodes;
    public UnsafeBuffer<int> WindowToSectorOffsets;
    public UnsafeBuffer<ConnectionPoint> ConnectionPoints;
    public UnsafeBuffer<ConnectionLink> ConnectionLinks;
    public UnsafeBuffer<IntegrationField> SectorIntegrationField;
    public UnsafeBuffer<bool> WindowInVisitedField;

    public UnsafeBuffer<int> WindowInVisitedQueue;
    public int WindowIntegrationQueueSize;

    //読み取り専用フィールド
    public int PortalPerWindow;
    public Vector3Int DimensionSize;
    public Vector3Int ChunkSize;
    public ReadOnlySpan<byte> CostField;
    public ReadOnlySpan<byte> GroundField;
    public ReadOnlyGroundHeightMap GroundHeightMap;


    public void Build()
    {
        ConfigureSectorNodes(new Vector3Byte((byte)ChunkSize.X, (byte)ChunkSize.Y, (byte)ChunkSize.Z));
        ConfigureWindowNodes(PortalPerWindow);
    }


    private void ConfigureSectorNodes(Vector3Byte chunkSize)
    {
        var iterableSectorToWindowOffsets = 0;
        for (int y = 0; y < DimensionSize.Y; y++)
        {
            for (int z = 0; z < DimensionSize.Z; z++)
            {
                for (int x = 0; x < DimensionSize.X; x++)
                {
                    var sectorIndex = WorldIndexUtilities.XZYToIndex(x, y, z, DimensionSize.X, DimensionSize.Z);
                    var sector = new Sector(chunkSize);
                    var sectorToWindowAmount = 6;
                    if (IsOnCorner(x, y, z))
                    {
                        sectorToWindowAmount = 3;
                    }
                    else if (IsOnEdge(x, y, z))
                    {
                        sectorToWindowAmount = 4;
                    }
                    else if (IsOnFace(x, y, z))
                    {
                        sectorToWindowAmount = 5;
                    }

                    SectorNodes[sectorIndex] =
                        new SectorNode(sector, sectorToWindowAmount, iterableSectorToWindowOffsets);
                    iterableSectorToWindowOffsets += sectorToWindowAmount;
                }
            }
        }
    }

    private void ConfigureWindowNodes(int portalPerWindow)
    {
        var portalOffsetJumpFactor = portalPerWindow;
        var costField = CostField;
        var groundField = GroundField;
        var groundHeightMap = GroundHeightMap;
        var visitedField = WindowInVisitedField;
        var windowInVisitedQueue = WindowInVisitedQueue;
        var windowInVisitedQueueSize = WindowIntegrationQueueSize;
        var dimensionSize = DimensionSize;
        var chunkSize = ChunkSize;
        var sectorFiledAmount = chunkSize.X * chunkSize.Y * chunkSize.Z;


        int windowNodeIndex = 0;
        //int iterableWindowToSectorOffsets = 0;
        for (int y = 0; y < dimensionSize.Y; y++)
        {
            for (int z = 0; z < dimensionSize.Z; z++)
            {
                for (int x = 0; x < dimensionSize.X; x++)
                {
                    var sectorIndex = WorldIndexUtilities.XZYToIndex(x, y, z, dimensionSize.X, dimensionSize.Z);
                    var sector = SectorNodes[sectorIndex].Sector;
                    var pos = new Vector3Int(x, y, z);
                    if (!IsOnTop(y))
                    {
                        var boundaryFace = GetUpperBoundaryFace(sector, pos);
                        WindowNodes[windowNodeIndex] =
                            new WindowNode(boundaryFace,
                                windowNodeIndex * portalOffsetJumpFactor,
                                GetPortalCountFor(boundaryFace, in groundField, in groundField, in groundHeightMap));
                        windowNodeIndex++;
                    }

                    if (!IsOnRight(x))
                    {
                        var boundaryFace = GetRightBoundaryFace(sector, pos);
                        WindowNodes[windowNodeIndex] =
                            new WindowNode(boundaryFace,
                                windowNodeIndex * portalOffsetJumpFactor,
                                GetPortalCountFor(boundaryFace, in costField, in groundField, in groundHeightMap));
                        windowNodeIndex++;
                    }
                }
            }
        }

        return;

        //上部接続空間 Y-up面を覆う
        BoundaryFace GetUpperBoundaryFace(Sector sector, Vector3Int pos)
        {
            var chunkCellPosition = WorldIndexUtilities.ChunkPositionToChunkCellPosition(pos, chunkSize);
            var minBoundary = new Vector3Int(chunkCellPosition.X, chunkCellPosition.Y + chunkSize.Y - 1,
                chunkCellPosition.Z);
            var maxBoundary = new Vector3Int(chunkCellPosition.X + chunkSize.X - 1,
                chunkCellPosition.Y + chunkSize.Y,
                chunkCellPosition.Z + chunkSize.Z - 1);
            return new BoundaryFace(minBoundary, maxBoundary);
        }

        //右部接続空間 X-up面を覆う
        BoundaryFace GetRightBoundaryFace(Sector sector, Vector3Int pos)
        {
            var chunkCellPosition = WorldIndexUtilities.ChunkPositionToChunkCellPosition(pos, chunkSize);
            var minBoundary = new Vector3Int(chunkCellPosition.X + chunkSize.X - 1, chunkCellPosition.Y,
                chunkCellPosition.Z);
            var maxBoundary = new Vector3Int(chunkCellPosition.X + chunkSize.X, chunkCellPosition.Y + chunkSize.Y - 1,
                chunkCellPosition.Z + chunkSize.Z - 1);
            return new BoundaryFace(minBoundary, maxBoundary);
        }


        int GetPortalCountFor(BoundaryFace boundaryFace, in ReadOnlySpan<byte> costField,
            in ReadOnlySpan<byte> groundField, in ReadOnlyGroundHeightMap groundHeightMap)
        {
            var worldSize = chunkSize * dimensionSize;
            var min = boundaryFace.Min;
            var max = boundaryFace.Max;
            var width = max.X - min.X + 1;
            var height = max.Y - min.Y + 1;
            var depth = max.Z - min.Z + 1;
            var log2ChunkSizeX = WorldIndexUtilities.GetLog2(chunkSize.X);
            var log2ChunkSizeY = WorldIndexUtilities.GetLog2(chunkSize.Y);
            var log2ChunkSizeZ = WorldIndexUtilities.GetLog2(chunkSize.Z);

            var upJumpFactor = WorldIndexUtilities.YJumpFactor(chunkSize.X, chunkSize.Z);
            var rightJumpFactor = WorldIndexUtilities.XJumpFactor(chunkSize.X, chunkSize.Z);
            var forwardJumpFactor = WorldIndexUtilities.ZJumpFactor(chunkSize.X, chunkSize.Z);
            visitedField.Clear();
            windowInVisitedQueueSize = 0;

            int minSectorStartIndex =
                WorldIndexUtilities.WorldXZYToChunkStartIndex(min.X, min.Y, min.Z,
                    log2ChunkSizeX, log2ChunkSizeY, log2ChunkSizeZ, dimensionSize);
            int maxSectorStartIndex =
                WorldIndexUtilities.WorldXZYToChunkStartIndex(max.X, max.Y, max.Z,
                    log2ChunkSizeX, log2ChunkSizeY, log2ChunkSizeZ, dimensionSize);

            //ZY平面に対してのポータル数
            if (boundaryFace.IsLeftOrRight())
            {
                int leftFindStartIndex =
                    WorldIndexUtilities.WorldXZYToIndex(min.X, max.Y, min.Z,
                        log2ChunkSizeX, log2ChunkSizeY, log2ChunkSizeZ, dimensionSize);
                int rightFindStartIndex =
                    WorldIndexUtilities.WorldXZYToIndex(max.X, max.Y, max.Z,
                        log2ChunkSizeX, log2ChunkSizeY, log2ChunkSizeZ, dimensionSize);
                int portalAmount = 0;
                for (int y = max.Y - 1; y >= 0; y--)
                {
                    for (int z = min.Z; z < min.Z + chunkSize.Z; z++)
                    {
                        var leftY = groundHeightMap.Read(leftFindStartIndex + z * forwardJumpFactor);
                        var searchIndex = WorldIndexUtilities.WorldXZYToChunkLocalIndex(min.X, leftY, z, log2ChunkSizeX,
                            log2ChunkSizeY, log2ChunkSizeZ);
                        windowInVisitedQueueSize = 0;
                        if (visitedField[searchIndex]) continue;
                        Enqueue(searchIndex, in costField);
                        while (windowInVisitedQueueSize > 0)
                        {
                            var current = windowInVisitedQueue[--windowInVisitedQueueSize];
                            CheckNeighbor(min.X, leftY, z, current, in costField, in groundField);
                        }
                    }
                }

                return portalAmount;
            }
            //XZ平面に対してのポータル数
            else if (boundaryFace.IsTopOrBottom())
            {
            }
            //XY平面に対してのポータル数
            else if (boundaryFace.IsFrontOrBack())
            {
                return 10;
            }

            throw new ArgumentException();


            void Enqueue(int index, in ReadOnlySpan<byte> costField)
            {
                if (costField[index] == 0 && !visitedField[index])
                {
                    windowInVisitedQueue[windowInVisitedQueueSize++] = index;
                    visitedField[index] = true;
                }
            }

            bool CheckSpace(int x, int y, int z, int index, in ReadOnlySpan<byte> costField)
            {
                if (x < 0 || y < 0 || z < 0 || x >= worldSize.X || y >= worldSize.Y || z >= worldSize.Z)
                {
                    return false;
                }

                if (x < min.X || x > max.X || y < min.Y || y > max.Y || z < min.Z || z > max.Z)
                {
                    return false;
                }

                if (costField[index] == 1)
                {
                    return false;
                }

                return true;
            }

            void CheckNeighbor(int x, int y, int z, int index, in ReadOnlySpan<byte> costField,
                in ReadOnlySpan<byte> groundField)
            {
                if (!CheckSpace(x, y, z, index, in costField))
                {
                    return;
                }

                if (groundField[index] > y + 1)
                {
                    return;
                }

                Enqueue(index, in costField);

                CheckNeighbor(x - 1, y, z, index - rightJumpFactor, in costField, in groundField);
                CheckNeighbor(x + 1, y, z, index + rightJumpFactor, in costField, in groundField);
                CheckNeighbor(x, y, z - 1, index - forwardJumpFactor, in costField, in groundField);
                CheckNeighbor(x, y, z + 1, index + forwardJumpFactor, in costField, in groundField);
                CheckNeighbor(x, y - 1, z, index - upJumpFactor, in costField, in groundField);
                CheckNeighbor(x, y + 1, z, index + upJumpFactor, in costField, in groundField);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOnCorner(int x, int y, int z)
    {
        bool leftOrRight = IsOnLeft(x) || IsOnRight(x);
        bool topOrBottom = IsOnTop(y) || IsOnBottom(y);
        bool frontOrBack = IsOnFront(z) || IsOnBack(z);
        return leftOrRight && topOrBottom && frontOrBack;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOnFace(int x, int y, int z)
    {
        return IsOnBottom(y) || IsOnTop(y) || IsOnLeft(x) || IsOnRight(x) || IsOnFront(z) || IsOnBack(z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOnEdge(int x, int y, int z)
    {
        bool leftOrRight = IsOnLeft(x) || IsOnRight(x);
        bool topOrBottom = IsOnTop(y) || IsOnBottom(y);
        bool frontOrBack = IsOnFront(z) || IsOnBack(z);
        return (leftOrRight && topOrBottom) || (leftOrRight && frontOrBack) || (topOrBottom && frontOrBack);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsOnBottom(int y)
    {
        return y == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsOnTop(int y)
    {
        return y == DimensionSize.Y - 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsOnLeft(int x)
    {
        return x == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsOnRight(int x)
    {
        return x == DimensionSize.X - 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsOnFront(int z)
    {
        return z == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsOnBack(int z)
    {
        return z == DimensionSize.Z - 1;
    }
}