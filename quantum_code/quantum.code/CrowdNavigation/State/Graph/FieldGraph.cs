using System;
using Photon.Deterministic;
using Quantum.FF.Collections;

namespace Quantum.FF;

public ref struct FieldGraphInitializationData
{
    public Native.Allocator Allocator;
    public Vector3Int DimensionSize;
    public Vector3Int ChunkSize;
}

public class FieldGraph
{
    private UnsafeBuffer<SectorNode> _sectorNodes;
    private UnsafeBuffer<int> _sectorToWindowOffsets;
    private UnsafeBuffer<WindowNode> _windowNodes;
    private UnsafeBuffer<int> _windowToSectorOffsets;
    private UnsafeBuffer<ConnectionPoint> _connectionPoints;
    private UnsafeBuffer<ConnectionLink> _connectionLinks;

    private UnsafeBuffer<IntegrationField> _sectorIntegrationFields;
    private UnsafeBuffer<bool> _windowInVisitedField;
    private UnsafeBuffer<int> _windowInVisitedQueue;
    private int _windowInVisitedQueueSize;
    private int _portalPerWindow;
    public FieldGraph(in FieldGraphInitializationData data)
    {
        int sectorAmount = data.DimensionSize.X * data.DimensionSize.Y * data.DimensionSize.Z;
        int cellAmount = data.ChunkSize.X * data.ChunkSize.Y * data.ChunkSize.Z;
        int windowAmount = 3 * sectorAmount -
                           2 * (data.DimensionSize.X * data.DimensionSize.Y) -
                           2 * (data.DimensionSize.X * data.DimensionSize.Z) -
                           2 * (data.DimensionSize.Y * data.DimensionSize.Z);

        int windowToOffsetsLength = windowAmount * 2;
        int sectorToOffsetsLength = sectorAmount * 2;
        int portalPerWindow = (data.ChunkSize.Y) / 4;
        int connectionAmount = windowAmount * portalPerWindow;
        int connectionLinkAmount = connectionAmount * (portalPerWindow * 8 - 2);

        var allocator = data.Allocator;
        _portalPerWindow = portalPerWindow;
        _sectorNodes = new UnsafeBuffer<SectorNode>(sectorAmount, allocator);
        _sectorToWindowOffsets = new UnsafeBuffer<int>(sectorToOffsetsLength, allocator);
        _windowNodes = new UnsafeBuffer<WindowNode>(windowAmount, allocator);
        _windowToSectorOffsets = new UnsafeBuffer<int>(windowToOffsetsLength, allocator);

        _connectionPoints = new UnsafeBuffer<ConnectionPoint>(sectorAmount * 6, allocator);
        _connectionLinks = new UnsafeBuffer<ConnectionLink>(connectionLinkAmount, allocator);

        _sectorIntegrationFields = new UnsafeBuffer<IntegrationField>(cellAmount, allocator);
        _windowInVisitedField = new UnsafeBuffer<bool>(cellAmount, allocator);
        _windowInVisitedQueue = new UnsafeBuffer<int>(cellAmount, allocator);
        _windowInVisitedQueueSize = 0;
    }
    
    public void GraphBuild(in FlowFieldData flowFieldData)
    {
        var builder = GenerateGraphBuilder(flowFieldData);
        builder.Build();
    }
    public FieldGraphBuilder GenerateGraphBuilder(in FlowFieldData flowFieldData)
    {
        var builder = new FieldGraphBuilder()
        {
            SectorNodes = _sectorNodes,
            SectorToWindowOffsets = _sectorToWindowOffsets,
            WindowNodes = _windowNodes,
            WindowToSectorOffsets = _windowToSectorOffsets,
            ConnectionPoints = _connectionPoints,
            ConnectionLinks = _connectionLinks,
            SectorIntegrationField = _sectorIntegrationFields,
            WindowInVisitedField = _windowInVisitedField,
            WindowInVisitedQueue = _windowInVisitedQueue,
            WindowIntegrationQueueSize = _windowInVisitedQueueSize,
            PortalPerWindow = _portalPerWindow,
            DimensionSize = flowFieldData.DimensionSize,
            ChunkSize = flowFieldData.ChunkSize,
            CostField = flowFieldData.CostField.AsSpan(),
            GroundField = flowFieldData.GroundField.AsSpan(),
            GroundHeightMap = new ReadOnlyGroundHeightMap(flowFieldData.GroundHeightMap.Data.AsSpan()),
        };
        
        return builder;
    }
    public void Free(Native.Allocator allocator)
    {
        _sectorNodes.Free(allocator);
        _sectorToWindowOffsets.Free(allocator);
        _windowNodes.Free(allocator);
        _windowToSectorOffsets.Free(allocator);
        _connectionPoints.Free(allocator);
        _connectionLinks.Free(allocator);
        _sectorIntegrationFields.Free(allocator);
        _windowInVisitedField.Free(allocator);
        _windowInVisitedQueue.Free(allocator);
    }
}