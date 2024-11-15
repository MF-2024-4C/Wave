using Quantum.FF;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Wave.FlowField.Editor
{
    public static class FlowFieldCostBakerMenu
    {
        [MenuItem("Quantum/FlowField/Bake Ground")]
        public static void Bake()
        {
            
            var flowFieldSurface = Object.FindAnyObjectByType<FlowFieldSurface>();
            if (flowFieldSurface == null)
            {
                Debug.LogError("No FlowFieldSurface found in scene");
                return;
            }

            var flowFieldCostBaker = new FlowFieldBaker();
            flowFieldCostBaker.Bake(flowFieldSurface);
        }
        
        [MenuItem("Quantum/FlowField/Bake Costs")]
        public static void BakeCosts()
        {
            var flowFieldSurface = Object.FindAnyObjectByType<FlowFieldSurface>();
            if (flowFieldSurface == null)
            {
                Debug.LogError("No FlowFieldSurface found in scene");
                return;
            }

            var flowFieldCostBaker = new FlowFieldBaker();
            flowFieldCostBaker.CostFieldBake(flowFieldSurface);
        }

        public static void BakeGround()
        {
        }
        
        

        public static Vector3Int ToChunkPosition(int x, int y, int z, int chunkPowerX, int chunkPowerY, int chunkPowerZ)
        {
            return new Vector3Int(
                ToChunkPosition(x, chunkPowerX),
                ToChunkPosition(y, chunkPowerY),
                ToChunkPosition(z, chunkPowerZ)
            );
        }

        private static int ToChunkPosition(int val, int chunkPower)
        {
            return (val >> chunkPower);
        }
    }
}