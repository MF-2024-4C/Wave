using System;
using Photon.Deterministic;
using Quantum.FF;
using Quantum.Inspector;

namespace Quantum;

public partial class FlowFieldData
{
    public int DimensionX;
    public int DimensionY;
    public int DimensionZ;
    public Vector3Int DimensionSize => new Vector3Int(DimensionX, DimensionY, DimensionZ);
    
    public int ChunkSizeX = 32;
    public int ChunkSizeY = 128;
    public int ChunkSizeZ = 32;
    public Vector3Int ChunkSize => new Vector3Int(ChunkSizeX, ChunkSizeY, ChunkSizeZ);
    
    public int SizeX => DimensionX * ChunkSizeX;
    public int SizeY => DimensionY * ChunkSizeY;
    public int SizeZ => DimensionZ * ChunkSizeZ;
    
    public int VolumeSize => SizeX * SizeY * SizeZ;

    public FP CellSize;
    public FPVector3 Origin;
    
    [HideInInspector] public byte[] CostField;
    [HideInInspector] public byte[] GroundField;
    
    [HideInInspector] public GroundHeightMapManaged GroundHeightMap;
    
}

public partial class CrowdNavigationData
{
    public FP MaxSlope = FP._10 * 4;
    public FP MaxHeight = FP._10;
    public FP Radius = FP._0_50;
    public Baked BakedData;
}

[Serializable]
public struct Baked
{
    public int Width;
    public int Height;
    
}