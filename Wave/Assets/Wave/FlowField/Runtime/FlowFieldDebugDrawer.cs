using System;
using Quantum.CrowdNavigation.Utilities;
using UnityEditor;
using UnityEngine;

namespace Wave.FlowField
{
    public class FlowFieldDebugDrawer : MonoBehaviour
    {
        [Flags]
        private enum FieldDrawMode
        {
            None = 0,
            Empty = 1,
            Hit = 2,
            Ground = 4,
        }

        private enum FieldDrawShape
        {
            None,
            WireCube,
            CenterSphere,
        }

        [SerializeField] private FlowFieldDataAsset _flowFieldDataAsset;
        [SerializeField] private FieldDrawMode DrawCostField = FieldDrawMode.None;
        [SerializeField] private FieldDrawShape DrawShape = FieldDrawShape.WireCube;

        [SerializeField] private bool DrawPosition = false;
        [SerializeField] private bool DrawGroundY = false;

        [SerializeField] private Vector3Int DrawChunkIndex = Vector3Int.zero;

        [SerializeField] private Vector3Int DrawFrequency = new Vector3Int(1, 1, 1);
        [SerializeField] private Vector2Int MinMaxY = new Vector2Int(0, 16);

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            DrawChunkIndex.Clamp(Vector3Int.zero, new Vector3Int(
                _flowFieldDataAsset.Settings.SizeX / _flowFieldDataAsset.Settings.ChunkSizeX - 1,
                _flowFieldDataAsset.Settings.SizeY / _flowFieldDataAsset.Settings.ChunkSizeY - 1,
                _flowFieldDataAsset.Settings.SizeZ / _flowFieldDataAsset.Settings.ChunkSizeZ - 1
            ));

            if (DrawCostField != FieldDrawMode.None)
            {
                OnDrawCostField();
            }
        }


        private void OnDrawCostField()
        {
            var flowFieldData = _flowFieldDataAsset.Settings;
            var cellSize = flowFieldData.CellSize;
            var origin = flowFieldData.Origin;
            var costField = flowFieldData.CostField;
            var groundField = flowFieldData.GroundField;
            var groundHeightMap = flowFieldData.GroundHeightMap;
            var sizeX = flowFieldData.ChunkSizeX;
            var sizeY = flowFieldData.ChunkSizeY;
            var sizeZ = flowFieldData.ChunkSizeZ;

            var log2SizeX = WorldIndexUtilities.GetLog2(sizeX);
            var log2SizeY = WorldIndexUtilities.GetLog2(sizeY);
            var log2SizeZ = WorldIndexUtilities.GetLog2(sizeZ);
            
            var worldSize = new Vector3(
                flowFieldData.SizeX,
                flowFieldData.SizeY,
                flowFieldData.SizeZ
            );
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(origin.ToUnityVector3() + worldSize * cellSize.AsFloat / 2,
                worldSize * cellSize.AsFloat);

            var chunkPosition = DrawChunkIndex * new Vector3Int(sizeX, sizeY, sizeZ);
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(
                new Vector3(chunkPosition.x, chunkPosition.y, chunkPosition.z) * cellSize.AsFloat +
                origin.ToUnityVector3() + new Vector3(sizeX, sizeY, sizeZ) * cellSize.AsFloat / 2,
                new Vector3(sizeX, sizeY, sizeZ) * cellSize.AsFloat);

            if (MinMaxY.y == 0) MinMaxY.y = sizeY;
            if (MinMaxY.y > sizeY) MinMaxY.y = sizeY;
            if (MinMaxY.x < 0) MinMaxY.x = 0;
            if (MinMaxY.x > sizeY) MinMaxY.x = sizeY;
            if (MinMaxY.x == MinMaxY.y) MinMaxY.y = MinMaxY.x + 1;
            if (MinMaxY.x > MinMaxY.y)
            {
                (MinMaxY.x, MinMaxY.y) = (MinMaxY.y, MinMaxY.x);
            }

            for (int y = MinMaxY.x; y < MinMaxY.y; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    for (int x = 0; x < sizeX; x++)
                    {
                        if (x % DrawFrequency.x != 0 || y % DrawFrequency.y != 0 || z % DrawFrequency.z != 0) continue;
                        var blockX = x + chunkPosition.x;
                        var blockY = y + chunkPosition.y;
                        var blockZ = z + chunkPosition.z;

                        var index = 
                        WorldIndexUtilities.WorldXZYToIndex(blockX, blockY, blockZ,
                            log2SizeX, log2SizeY, log2SizeZ,
                            flowFieldData.DimensionSize);
                        
                        var cost = costField[index];
                        var isGround = groundField[index] == 1;
                        var isVisible = (DrawCostField.HasFlag(FieldDrawMode.Empty) && cost == 0) ||
                                        (DrawCostField.HasFlag(FieldDrawMode.Hit) && cost > 0) ||
                                        (DrawCostField.HasFlag(FieldDrawMode.Ground) && isGround);

                        if (!isVisible) continue;


                        if (cost == 1)
                        {
                            Gizmos.color = Color.red;
                        }
                        else if (DrawCostField.HasFlag(FieldDrawMode.Ground) && isGround)
                        {
                            Gizmos.color = Color.green;
                        }
                        else
                        {
                            Gizmos.color = Color.blue;
                        }

                        var center = new Vector3(blockX, blockY, blockZ) * cellSize.AsFloat + origin.ToUnityVector3();
                        DrawCell(center, cellSize.AsFloat);
                        var labelCenter = center + Vector3.up * cellSize.AsFloat;
                        if (DrawPosition) Handles.Label(labelCenter, $"{blockX}, {blockY}, {blockZ}:index:{index}");
                        if (DrawGroundY)
                            Handles.Label(labelCenter,
                                $"Ground:{groundHeightMap.Read(index)}");
                    } 
                }
            }
        }

        private void DrawCell(Vector3 center, float size)
        {
            switch (DrawShape)
            {
                case FieldDrawShape.WireCube:
                    Gizmos.DrawWireCube(center, new Vector3(size, size, size));
                    break;
                case FieldDrawShape.CenterSphere:
                    Gizmos.DrawSphere(center, size / 4);
                    break;
                case FieldDrawShape.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
#endif
    }
}