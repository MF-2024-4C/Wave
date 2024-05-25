using Quantum;
using Quantum.Core;
using UnityEngine;

[CreateAssetMenu(menuName = "Quantum/SystemConfig", order = Quantum.EditorDefines.AssetMenuPriorityStart + 468)]
public partial class SystemConfigAsset : AssetBase {
    public Quantum.SystemConfig Settings;

    public override Quantum.AssetObject AssetObject => Settings;


    public override void Reset()
    {
        Settings = new Quantum.SystemConfig();
        Settings.AddSystem<CullingSystem2D>();
        Settings.AddSystem<CullingSystem3D>();
        Settings.AddSystem<PhysicsSystem2D>();
        Settings.AddSystem<PhysicsSystem3D>();
        Settings.AddSystem<NavigationSystem>();
        Settings.AddSystem<EntityPrototypeSystem>();
        base.Reset();
    }
}

public static partial class SystemConfigAssetExts {
    public static SystemConfigAsset GetUnityAsset(this SystemConfig data) {
        return data == null ? null : UnityDB.FindAsset<SystemConfigAsset>(data);
    }
}