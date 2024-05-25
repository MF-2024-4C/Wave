using Quantum.QuantumDemo;
using Quantum.Wave.Zombie;

namespace Quantum {
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;

  /// <summary>
  /// Responsible for instantiating Quantum systems on simualtion start.
  /// User systems can be added by adding a <see cref="SystemsConfig"/> to the <see cref="RuntimeConfig"/>.
  /// Or adding them in the user callback <see cref="AddSystemsUser"/>.
  /// </summary>
  public static partial class DeterministicSystemSetup {
    static partial void AddSystemsUser(ICollection<SystemBase> systems, RuntimeConfig gameConfig, SimulationConfig simulationConfig, SystemConfig systemsConfig);

    public static ICollection<SystemBase> CreateSystems(RuntimeConfig gameConfig, SimulationConfig simulationConfig, SystemConfig systemsConfig) {
      var systems = CallLegacySystemSetup(gameConfig, simulationConfig);
      if (systems != null) {
        return systems;
      }

      if (systemsConfig != null) {
        systems = SystemConfig.CreateSystems(systemsConfig);
      } else {
        systems = new List<SystemBase>() {
          new Core.CullingSystem2D(),
          new Core.CullingSystem3D(),
          new Core.PhysicsSystem2D(),
          new Core.PhysicsSystem3D(),
          new Core.NavigationSystem(),
          new Core.EntityPrototypeSystem(),
          new Core.PlayerConnectedSystem(),
          new PlayerSpawnSystem(),
          new MovementSystem(),
          new ZombieSystem()
        };
      }

      var debugSystem = Core.DebugCommand.CreateSystem();
      if (debugSystem != null) {
        systems.Add(debugSystem);
      }

      AddSystemsUser(systems, gameConfig, simulationConfig, systemsConfig);

      return systems;
    }

    private static ICollection<SystemBase> CallLegacySystemSetup(RuntimeConfig gameConfig, SimulationConfig simulationConfig) {
      var systemSetupType = TypeUtils.FindType("SystemSetup");
      if (systemSetupType == null) {
        return null;
      }
      
      var createSystemsMethodInfo = systemSetupType.GetMethod("CreateSystems", BindingFlags.Public | BindingFlags.Static);
      if (createSystemsMethodInfo == null) {
        return null;
      }

      var systemArray = (SystemBase[])createSystemsMethodInfo.Invoke(null, new object[] { gameConfig, simulationConfig });
      if (systemArray == null || systemArray.Length == 0) {
        return null;
      }

      return systemArray.ToList();
    }
  }
}