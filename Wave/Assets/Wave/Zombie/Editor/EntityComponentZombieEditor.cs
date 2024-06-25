using GizmosUtility.Editor;
using UnityEditor;
using UnityEngine;

namespace Wave.Zombie.Editor
{
    [CustomEditor(typeof(EntityComponentZombie))]
    public class EntityComponentZombieEditor : UnityEditor.Editor
    {
        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
        private static void DrawGizmos(EntityComponentZombie zombie, GizmoType gizmoType)
        {
            Gizmos.color = Color.red;
            
            var zombieSpecAsset = zombie.Prototype.Spec;
            var zombieSpec = UnityDB.FindAsset<ZombieSpecAsset>(zombieSpecAsset.Id).Settings;
            var radius = zombieSpec.DormantSearchDistance;
            var center = zombie.transform.position;
            GizmosExtensions.DrawCircle(center, radius.AsFloat);
        }
    }
}