using UnityEngine;
using UnityEngine.UI;

namespace Wave.UI.Effect.Shear
{
    [AddComponentMenu("UI/Effects/ShearEffect")]
    public class ShearEffect : BaseMeshEffect
    {
        [SerializeField] private float _shearAngle = 0f; // せん断角度

        public float ShearAngle
        {
            get => _shearAngle;
            set
            {
                _shearAngle = value;
                if (graphic != null)
                {
                    graphic.SetVerticesDirty();
                }
            }
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
                return;

            var shearRadians = _shearAngle * Mathf.Deg2Rad;
            var tanShear = Mathf.Tan(shearRadians);

            var vertex = new UIVertex();
            for (var i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vertex, i);
                vertex.position = new Vector3(
                    vertex.position.x,
                    vertex.position.y + tanShear * vertex.position.x,
                    vertex.position.z
                );
                vh.SetUIVertex(vertex, i);
            }
        }
    }
}