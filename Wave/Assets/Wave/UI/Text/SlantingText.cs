using TMPro;
using UnityEngine;

namespace Wave.UI.Text
{
    [ExecuteAlways]
    public class SlantingText : MonoBehaviour
    {
        private TMP_Text _textMeshPro;
        [SerializeField] private bool _isEnable = true;
        [SerializeField] private float _angle = 5;

        private void OnEnable()
        {
            _textMeshPro = gameObject.GetComponent<TMP_Text>();
            _textMeshPro.OnPreRenderText += TextUpdate;
            TextUpdate(_textMeshPro.textInfo);
        }
        
        private void OnDisable()
        {
            _textMeshPro.OnPreRenderText -= TextUpdate;
            _textMeshPro.ForceMeshUpdate();
        }

        private void TextUpdate(TMP_TextInfo textInfo)
        {
            if (!_isEnable) return;
            if (textInfo.characterCount == 0)
            {
                return;
            }

            var offsetHeight = Mathf.Sin(Mathf.Deg2Rad * _angle);

            for (var index = 0; index < textInfo.characterCount; index++)
            {
                var charaInfo = textInfo.characterInfo[index];
                if (!charaInfo.isVisible)
                {
                    continue;
                }

                var materialIndex = charaInfo.materialReferenceIndex;
                var vertexIndex = charaInfo.vertexIndex;
                var destVertices = textInfo.meshInfo[materialIndex].vertices;
                var charaHeight = destVertices[vertexIndex + 1].y - destVertices[vertexIndex + 0].y;
                var offsetY = (offsetHeight * charaHeight * index);
                var offset = new Vector3(0, -offsetY, 0);

                destVertices[vertexIndex + 0] += offset;
                destVertices[vertexIndex + 1] += offset;
                destVertices[vertexIndex + 2] += offset;
                destVertices[vertexIndex + 3] += offset;
            }

            for (var i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                _textMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }
        }
    }
}