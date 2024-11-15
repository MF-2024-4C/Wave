using Photon.Deterministic;
using Quantum.FF.Collections;

namespace Quantum.FF;

public ref struct ChunkInitializationData
{
    public Vector3Int ChunkSize;
    public Vector3Int ChunkPosition;
    public UnsafeSlice<byte> CostField;
    public UnsafeSlice<byte> GroundField;
}

public struct FFChunk
{
    public Vector3Int ChunkPosition { get; }

    public Vector3Int ChunkSize { get; }

    private GroundHeightMap _groundHeightMap;
    private TerrainArray<byte> _costField;
    private TerrainArray<ushort> _flowField;

    public FFChunk(in ChunkInitializationData chunkInitializationData, Native.Allocator allocator)
    {
        ChunkSize = chunkInitializationData.ChunkSize;
        ChunkPosition = chunkInitializationData.ChunkPosition;
        
        _costField = new TerrainArray<byte>(ChunkSize.X, ChunkSize.Y, ChunkSize.Z, allocator);
        _groundHeightMap = new GroundHeightMap(ChunkSize.X, ChunkSize.Y, ChunkSize.Z, allocator);
        
        _flowField = new TerrainArray<ushort>(ChunkSize.X, ChunkSize.Y, ChunkSize.Z, allocator);
        
    }


    public void Free(Native.Allocator allocator)
    {
        _flowField.Free(allocator);
        _costField.Free(allocator);
        _groundHeightMap.Free(allocator);
    }

    public ushort GetField(Vector3Int position)
    {
        return _flowField.Get(position.X, position.Y, position.Z);
    }
}