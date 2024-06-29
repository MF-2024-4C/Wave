using System.Collections.Generic;

namespace Quantum.FlowField
{
    public class FlowFieldMapBaker : MapDataBakerCallback
    {
        private List<FPRect> _collisionRects = new();

        public override void OnBeforeBake(MapData data)
        {
        }

        public override void OnBake(MapData data)
        {
        }
    }
}