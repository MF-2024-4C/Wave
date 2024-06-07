namespace Quantum.Wave.Weapon;

public unsafe class PlayerSpawnSystem : SystemSignalsOnly, ISignalOnPlayerDataSet
{
    public void OnPlayerDataSet(Frame frame, PlayerRef player)
    {
        var data = frame.GetPlayerData(player);

        var prototype = frame.FindAsset<EntityPrototype>(data.CharacterPrototype.Id);

        var entity = frame.Create(prototype);

        var playerLink = new PlayerLink()
        {
            Player = player,
        };

        frame.Add(entity, playerLink);

        Log.Info("PlayerSpawnSystem.OnPlayerDataSet");

        //スーポン地点をずらすように
        if (frame.Unsafe.TryGetPointer<Transform3D>(entity, out var transform))
        {
            transform->Position.X = (int)player;
        }

        //武器を持たせる
        if (frame.Unsafe.TryGetPointer<WeaponInventory>(entity, out var inventory))
        {
            if (frame.Unsafe.TryGetPointer<WeaponInventorySetup>(entity, out var weaponSetup))
            {
                var weaponPrototype = frame.FindAsset<EntityPrototype>(weaponSetup->PrimaryWeaponData.Id);
                if (weaponPrototype != null) inventory->primary = frame.Create(weaponPrototype);

                weaponPrototype = frame.FindAsset<EntityPrototype>(weaponSetup->SecondaryWeaponData.Id);
                if (weaponPrototype != null) inventory->secondary = frame.Create(weaponPrototype);

                weaponPrototype = frame.FindAsset<EntityPrototype>(weaponSetup->TertiaryWeaponData.Id);
                if (weaponPrototype != null) inventory->tertiary = frame.Create(weaponPrototype);
            }
        }


        /*
        if (frame.Unsafe.TryGetPointerSingleton<PlayersManager>(out var manager))
        {
            manager->Players[(int)player] = entity;
        }
        */


        /*
        if ( frame.Unsafe.TryGetPointer<Transform3D>(entity, out var transform) )
        {
            transform->Position = new FPVector3(0, 0, 0);
        }
        */
    }
}