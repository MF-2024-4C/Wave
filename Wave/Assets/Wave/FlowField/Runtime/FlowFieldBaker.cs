using Quantum.CrowdNavigation.Utilities;
using Quantum.FF;
using Unity.Profiling;
using UnityEngine;

namespace Wave.FlowField
{
    public class FlowFieldBaker
    {
        private static ProfilerMarker _costFieldBakeMarker = new("CostFieldBake");
        private static ProfilerMarker _groundFieldBakeMarker = new("GroundFieldBake");
        private static ProfilerMarker _groundHeightLayerBakeMarker = new("GroundHeightLayerBake");

        public void Bake(FlowFieldSurface flowFieldSurface)
        {
            CostFieldBake(flowFieldSurface);
            GroundFieldBake(flowFieldSurface);
        }

        public void CostFieldBake(FlowFieldSurface flowFieldSurface)
        {
            using (_costFieldBakeMarker.Auto())
            {
                var settings = flowFieldSurface._flowFieldDataAsset.Settings;
                var cellSize = settings.CellSize.AsFloat;
                var halfCellSize = cellSize * 0.5f;
                var origin = settings.Origin.ToUnityVector3();
                settings.CostField = new byte[settings.SizeX * settings.SizeY * settings.SizeZ];

                var log2SizeX = WorldIndexUtilities.GetLog2(settings.ChunkSizeX);
                var log2SizeY = WorldIndexUtilities.GetLog2(settings.ChunkSizeY);
                var log2SizeZ = WorldIndexUtilities.GetLog2(settings.ChunkSizeZ);
                for (int y = 0; y < settings.SizeY; y++)
                {
                    for (int z = 0; z < settings.SizeZ; z++)
                    {
                        for (int x = 0; x < settings.SizeX; x++)
                        {
                            var index = WorldIndexUtilities.WorldXZYToIndex(x, y, z,
                                log2SizeX, log2SizeY, log2SizeZ,
                                settings.DimensionSize);
                            var blockX = x;
                            var blockY = y;
                            var blockZ = z;
                            var center = new Vector3(blockX, blockY, blockZ) * cellSize + origin;

                            settings.CostField[index] =
                                Physics.CheckBox(center,
                                    Vector3.one * halfCellSize, Quaternion.identity)
                                    ? (byte)1
                                    : (byte)0;
                        }
                    }
                }
            }
        }

        public void GroundFieldBake(FlowFieldSurface flowFieldSurface)
        {
            using (_groundFieldBakeMarker.Auto())
            {
                var settings = flowFieldSurface._flowFieldDataAsset.Settings;
                settings.GroundField = new byte[settings.SizeX * settings.SizeY * settings.SizeZ];
                settings.GroundHeightMap = new GroundHeightMapManaged();
                settings.GroundHeightMap.Initialize(settings.SizeX, settings.SizeY, settings.SizeZ);

                var log2SizeX = WorldIndexUtilities.GetLog2(settings.ChunkSizeX);
                var log2SizeY = WorldIndexUtilities.GetLog2(settings.ChunkSizeY);
                var log2SizeZ = WorldIndexUtilities.GetLog2(settings.ChunkSizeZ);

                for (int y = 1; y < settings.SizeY; y++)
                {
                    for (int z = 0; z < settings.SizeZ; z++)
                    {
                        for (int x = 0; x < settings.SizeX; x++)
                        {
                            var blockX = x;
                            var blockY = y;
                            var blockZ = z;
                            var index = WorldIndexUtilities.WorldXZYToIndex(x, y, z,
                                log2SizeX, log2SizeY, log2SizeZ,
                                settings.DimensionSize);
                            var bottomIndex = WorldIndexUtilities.WorldXZYToIndex(x, y - 1, z,
                                log2SizeX, log2SizeY, log2SizeZ,
                                settings.DimensionSize);
                            var topIndex = WorldIndexUtilities.WorldXZYToIndex(x, y + 1, z,
                                log2SizeX, log2SizeY, log2SizeZ,
                                settings.DimensionSize);

                            if (settings.CostField[index] == 0)
                            {
                                bool isGround = settings.CostField[bottomIndex] == 1;
                                bool isTopSpace = blockY + 1 < settings.SizeY && settings.CostField[topIndex] == 0;
                                if (isGround && isTopSpace)
                                {
                                    settings.GroundField[index] = 1;
                                    settings.GroundHeightMap.Write(index, y, settings.ChunkSizeX, settings.ChunkSizeZ);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void GroundHeightLayerBake(FlowFieldSurface flowFieldSurface)
        {
            using (_groundHeightLayerBakeMarker.Auto())
            {
                var settings = flowFieldSurface._flowFieldDataAsset.Settings;
            }
        }
    }
}