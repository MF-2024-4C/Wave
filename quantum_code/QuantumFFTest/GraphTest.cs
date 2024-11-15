using System;
using Photon.Deterministic;
using Quantum;
using Assert = Xunit.Assert;

namespace QuantumFFTest
{
    public class GraphTest : IDisposable
    {
        private Native.Allocator _allocator;

        public GraphTest()
        {
            _allocator = SessionContainer.CreateNativeAllocator();
        }

        public void Dispose()
        {
            _allocator.Dispose();
        }

        [Fact]
        public unsafe void GraphStructSizeTest()
        {
            Span<int> span = new Span<int>(new int[10]);
            var boundaryFaceSize = sizeof(BoundaryFace);
            Assert.Equal(Vector3Int.SIZE * 2, boundaryFaceSize);

            var windowNodeSize = sizeof(WindowNode);
            Assert.Equal(boundaryFaceSize + sizeof(int) * 2, windowNodeSize);
        }

        [Fact]
        public unsafe void GraphInitializationTest()
        {
            var allocator = _allocator;
            var dimensionSize = new Vector3Int(6, 1, 12);
            var chunkSize = new Vector3Int(32, 128, 32);
            var data = new FieldGraphInitializationData
            {
                Allocator = allocator,
                DimensionSize = dimensionSize,
                ChunkSize = chunkSize
            };
            var fieldGraph = new FieldGraph(data);
            var flowFieldData = new FlowFieldData()
            {
                DimensionX = dimensionSize.X,
                DimensionY = dimensionSize.Y,
                DimensionZ = dimensionSize.Z,
                ChunkSizeX = chunkSize.X,
                ChunkSizeY = chunkSize.Y,
                ChunkSizeZ = chunkSize.Z,
                CellSize = FP._0_50,
                Origin = FPVector3.Zero
            };
            var builder = fieldGraph.GenerateGraphBuilder(flowFieldData);
            Assert.True(builder.IsOnCorner(0, 0, 0));
            Assert.True(builder.IsOnCorner(dimensionSize.X - 1, 0, 0));
            Assert.True(builder.IsOnCorner(0, dimensionSize.Y - 1, 0));
            Assert.True(builder.IsOnCorner(0, 0, dimensionSize.Z - 1));
            Assert.True(builder.IsOnCorner(dimensionSize.X - 1, dimensionSize.Y - 1, 0));
            Assert.True(builder.IsOnCorner(dimensionSize.X - 1, 0, dimensionSize.Z - 1));
            Assert.True(builder.IsOnCorner(0, dimensionSize.Y - 1, dimensionSize.Z - 1));
            Assert.True(builder.IsOnCorner(dimensionSize.X - 1, dimensionSize.Y - 1, dimensionSize.Z - 1));
            Assert.False(builder.IsOnCorner(1, 1, 1));
            Assert.False(builder.IsOnCorner(0, 0, 1));
            Assert.False(builder.IsOnCorner(0, 1, 0));
            Assert.False(builder.IsOnCorner(1, 0, 0));
            
            Assert.True(builder.IsOnFace(1, 1, 0));
            Assert.True(builder.IsOnFace(1, 0, 1));
            Assert.True(builder.IsOnFace(0, 1, 1));
            Assert.False(builder.IsOnFace(1, 1, 1));
            
            Assert.True(builder.IsOnEdge(1, 0, 0));
            Assert.True(builder.IsOnEdge(0, 1, 0));
            Assert.True(builder.IsOnEdge(0, 0, 1));
            Assert.False(builder.IsOnEdge(1, 1, 0));
            Assert.False(builder.IsOnEdge(1, 0, 1));
            Assert.False(builder.IsOnEdge(0, 1, 1));
            Assert.False(builder.IsOnEdge(1, 1, 1));

            fieldGraph.Free(allocator);
        }
    }
}