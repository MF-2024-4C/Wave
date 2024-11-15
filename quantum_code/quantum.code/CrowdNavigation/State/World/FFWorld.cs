using System;
using Photon.Deterministic;
using Quantum.CrowdNavigation.Utilities;
using Quantum.FF.Collections;

namespace Quantum.FF;


public ref struct WorldInitializationData
{
    public Vector3Int DimensionSize;
    public FP CellSize;
    public Vector3Int ChunkSize;
    public UnsafeBuffer<byte> CostField;
    public UnsafeBuffer<byte> GroundField;
}

public unsafe class FFWorld : IDisposable
{
    private FP _cellSize;
    private Vector3Int _dimensionSize;

    private FFChunk[] _chunks;
    private Vector3Int _chunkSize;
    private Native.Allocator _allocator;

    public FFWorld(in WorldInitializationData initializationData, Native.Allocator allocator)
    {
        _cellSize = initializationData.CellSize;
        _dimensionSize = initializationData.DimensionSize;
        _allocator = allocator;
        
        _chunkSize = initializationData.ChunkSize;
        if (WorldIndexUtilities.IsPowerOfTwo(_chunkSize.X) == false ||
            WorldIndexUtilities.IsPowerOfTwo(_chunkSize.Y) == false ||
            WorldIndexUtilities.IsPowerOfTwo(_chunkSize.Z) == false)
        {
            throw new ArgumentException("Chunk size must be a power of two");
        }


        var dimensionCount = _dimensionSize.X * _dimensionSize.Y * _dimensionSize.Z;
        _chunks = new FFChunk[dimensionCount];
        for (int i = 0; i < dimensionCount; i++)
        {
            var position = WorldIndexUtilities.IndexToXZY(i, _dimensionSize);
            ChunkInitializationData chunkInitializationData = new ChunkInitializationData
            {
                ChunkSize = _chunkSize,
                ChunkPosition = position,
                CostField = new UnsafeSlice<byte>(initializationData.CostField, i * _chunkSize.X * _chunkSize.Y * _chunkSize.Z, _chunkSize.X * _chunkSize.Y * _chunkSize.Z),
                GroundField = new UnsafeSlice<byte>(initializationData.GroundField, i * _chunkSize.X * _chunkSize.Y * _chunkSize.Z, _chunkSize.X * _chunkSize.Y * _chunkSize.Z)
            };
            _chunks[i] = new FFChunk(chunkInitializationData, _allocator);
        }
    }

    public ref FFChunk GetChunk(Vector3Int position)
    {
        var index = WorldIndexUtilities.XZYToIndex(position.X, position.Y, position.Z, _dimensionSize.X,
            _dimensionSize.Z);
        return ref _chunks[index];
    }

    public void Dispose()
    {
        for (int i = 0; i < _chunks.Length; i++)
        {
            _chunks[i].Free(_allocator);
        }
    }
}