using UnityEditor;
using UnityEngine;
using GizmosUtility.Editor;

namespace Wave.Zombie.Editor
{
    [CustomEditor(typeof(EntityComponentZombieSpawnArea))]
    public class EntityComponentZombieSpawnAreaEditor : UnityEditor.Editor
    {
        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
        private static void DrawGizmos(EntityComponentZombieSpawnArea spawnArea, GizmoType gizmoType)
        {
            Gizmos.color = Color.green;
            var radius = spawnArea.Prototype.Radius.AsFloat;
            var center = spawnArea.transform.position;
//            GizmosExtensions.DrawWireCircle(center, radius);
            GizmosExtensions.DrawCircle(center, radius);
            Gizmos.DrawIcon(center, "ZombieSpawnArea.png", true);
        }
    }
}