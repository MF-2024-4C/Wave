using UnityEditor;
using UnityEngine;

namespace Quantum.Editor
{
    [CustomPropertyDrawer(typeof(AssetRefSystemConfig))]
    public class AssetRefSystemConfigPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(SystemConfigAsset));
        }
    }
}