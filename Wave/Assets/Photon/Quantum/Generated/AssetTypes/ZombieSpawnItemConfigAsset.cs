// <auto-generated>
// This code was auto-generated by a tool, every time
// the tool executes this code will be reset.
//
// If you need to extend the classes generated to add
// fields or methods to them, please create partial  
// declarations in another file.
// </auto-generated>

using Quantum;
using UnityEngine;

[CreateAssetMenu(menuName = "Quantum/BaseItemConfig/ZombieSpawnItemConfig", order = Quantum.EditorDefines.AssetMenuPriorityStart + 51)]
public partial class ZombieSpawnItemConfigAsset : BaseItemConfigAsset {
  public Quantum.ZombieSpawnItemConfig Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.ZombieSpawnItemConfig();
    }
    base.Reset();
  }
}

public static partial class ZombieSpawnItemConfigAssetExts {
  public static ZombieSpawnItemConfigAsset GetUnityAsset(this ZombieSpawnItemConfig data) {
    return data == null ? null : UnityDB.FindAsset<ZombieSpawnItemConfigAsset>(data);
  }
}
