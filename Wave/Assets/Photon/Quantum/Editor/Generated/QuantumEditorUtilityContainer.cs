// <auto-generated>
// This code was auto-generated by a tool, every time
// the tool executes this code will be reset.
//
// If you need to extend the classes generated to add
// fields or methods to them, please create partial  
// declarations in another file.
// </auto-generated>

#if UNITY_EDITOR
namespace Quantum.Editor {
  using Quantum.Prototypes;
  using UnityEditor;
  using UnityEngine;

  internal sealed class QuantumEditorUtilityContainer : ScriptableSingleton<QuantumEditorUtilityContainer> {

    public static new QuantumEditorUtilityContainer instance {
      get {
        var result = ScriptableSingleton<QuantumEditorUtilityContainer>.instance;
        result.hideFlags = HideFlags.None;
        return result;
      }
    }

    public KnownObjectsContainer ObjectsContainer = new KnownObjectsContainer();
    public FlatEntityPrototypeContainer PendingPrototype = new FlatEntityPrototypeContainer();

    [System.Serializable]
    public partial class KnownObjectsContainer : QuantumEditorUtility.SerializableObjectsContainerBase {
      public Quantum.WeaponData[] WeaponData = {};
      public Quantum.SimulationConfig[] SimulationConfig = {};
      public Quantum.BinaryData[] BinaryData = {};
      public Quantum.CharacterController2DConfig[] CharacterController2DConfig = {};
      public Quantum.CharacterController3DConfig[] CharacterController3DConfig = {};
      public Quantum.EntityPrototype[] EntityPrototype = {};
      public Quantum.EntityView[] EntityView = {};
      public Quantum.Map[] Map = {};
      public Quantum.NavMesh[] NavMesh = {};
      public Quantum.NavMeshAgentConfig[] NavMeshAgentConfig = {};
      public Quantum.PhysicsMaterial[] PhysicsMaterial = {};
      public Quantum.PolygonCollider[] PolygonCollider = {};
      public Quantum.TerrainCollider[] TerrainCollider = {};

    }
  }
}
#endif
