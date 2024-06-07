// <auto-generated>
// This code was auto-generated by a tool, every time
// the tool executes this code will be reset.
// </auto-generated>

namespace Quantum.Editor {
  using Quantum;
  using UnityEngine;
  using UnityEditor;

  [CustomPropertyDrawer(typeof(AssetRefBaseItemConfig))]
  public class AssetRefBaseItemConfigPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(BaseItemConfigAsset));
    }
  }

  [CustomPropertyDrawer(typeof(AssetRefInteractConfig))]
  public class AssetRefInteractConfigPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(InteractConfigAsset));
    }
  }

  [CustomPropertyDrawer(typeof(AssetRefItem1Config))]
  public class AssetRefItem1ConfigPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(Item1ConfigAsset));
    }
  }

  [CustomPropertyDrawer(typeof(AssetRefItem2Config))]
  public class AssetRefItem2ConfigPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(Item2ConfigAsset));
    }
  }

  [CustomPropertyDrawer(typeof(AssetRefPlayerConfig))]
  public class AssetRefPlayerConfigPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(PlayerConfigAsset));
    }
  }

  [CustomPropertyDrawer(typeof(AssetRefWeaponData))]
  public class AssetRefWeaponDataPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(WeaponDataAsset));
    }
  }

  [CustomPropertyDrawer(typeof(AssetRefZombieSpawnAreaConfig))]
  public class AssetRefZombieSpawnAreaConfigPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(ZombieSpawnAreaConfigAsset));
    }
  }

  [CustomPropertyDrawer(typeof(AssetRefZombieSpec))]
  public class AssetRefZombieSpecPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(ZombieSpecAsset));
    }
  }

  [CustomPropertyDrawer(typeof(Quantum.Prototypes.FireMode_Prototype))]
  [CustomPropertyDrawer(typeof(Quantum.Prototypes.WeaponType_Prototype))]
  [CustomPropertyDrawer(typeof(Quantum.Prototypes.ZombieState_Prototype))]
  [CustomPropertyDrawer(typeof(Quantum.Prototypes.InputButtons_Prototype))]
  partial class PrototypeDrawer {}
}
