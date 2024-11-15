using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum;
using Quantum.CrowdNavigation.Utilities;
using Quantum.FF.Collections;
using Assert = Xunit.Assert;

namespace QuantumFFTest
{
    public class WorldTest : IDisposable
    {
        private Native.Allocator _allocator;

        public WorldTest()
        {
            _allocator = SessionContainer.CreateNativeAllocator();
        }

        [Fact]
        public unsafe void WorldGenerateTest()
        {
            Vector3Int dimensions = new Vector3Int(5, 1, 8);
            FP cellSize = FP._0_50;
            Vector3Int chunkSize = new Vector3Int(32, 128, 32);
            UnsafeBuffer<byte> buffer = new UnsafeBuffer<byte>(chunkSize.X * chunkSize.Y * chunkSize.Z * dimensions.X * dimensions.Y * dimensions.Z, _allocator);
            var worldData = new WorldInitializationData
            {
                DimensionSize = dimensions,
                CellSize = cellSize,
                ChunkSize = chunkSize,
                CostField = buffer,
                GroundField = buffer
            };
            FFWorld world = new FFWorld(in worldData, _allocator);

            for (int y = 0; y < dimensions.Y; y++)
            {
                for (int z = 0; z < dimensions.Z; z++)
                {
                    for (int x = 0; x < dimensions.X; x++)
                    {
                        var pos = new Vector3Int(x, y, z);
                        var chunk = world.GetChunk(pos);
                        Assert.NotNull(chunk);
                        Assert.True(chunk.ChunkPosition == pos);
                    }
                }
            }
            
            world.Dispose();
        }


        public static IEnumerable<object[]> ChunkSizeData()
        {
            return new object[][]
            {
                new object[] { new Vector3Int(32, 128, 32) },
                new object[] { new Vector3Int(16, 128, 16) },
            };
        }

        [Theory]
        [MemberData(nameof(ChunkSizeData))]
        public void IndexToXZYTest(Vector3Int chunkSize)
        {
            var v0 = WorldIndexUtilities.IndexToXZY(0, chunkSize);
            var v1 = WorldIndexUtilities.IndexToXZY(1, chunkSize);
            var v2 = WorldIndexUtilities.IndexToXZY(chunkSize.X, chunkSize);

            Assert.True(v0 is { X: 0, Y: 0, Z: 0 });
            Assert.True(v1 is { X: 1, Y: 0, Z: 0 });
            Assert.True(v2 is { X: 0, Y: 0, Z: 1 });


            var chunkPowerX = WorldIndexUtilities.GetLog2(chunkSize.X);
            var chunkPowerXZ = WorldIndexUtilities.GetLog2(chunkSize.X * chunkSize.Z);

            var v3 = WorldIndexUtilities.IndexToXZY(0, chunkPowerX, chunkPowerXZ, chunkSize.Z);
            var v4 = WorldIndexUtilities.IndexToXZY(1, chunkPowerX, chunkPowerXZ, chunkSize.Z);
            var v5 = WorldIndexUtilities.IndexToXZY(chunkSize.X, chunkPowerX, chunkPowerXZ, chunkSize.Z);

            Assert.True(v3 is { X: 0, Y: 0, Z: 0 });
            Assert.True(v4 is { X: 1, Y: 0, Z: 0 });
            Assert.True(v5 is { X: 0, Y: 0, Z: 1 });
        }

        public void Dispose()
        {
            _allocator.Dispose();
        }
    }
}