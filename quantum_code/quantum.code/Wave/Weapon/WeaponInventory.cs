using Quantum.Player;

namespace Quantum.Wave.Weapon;

public unsafe class WeaponInventorySystem : SystemMainThreadFilter<WeaponInventorySystem.GunHolderFilter>
{
    public struct GunHolderFilter
    {
        public EntityRef Entity;
        public WeaponInventory* Inventory;
        public Transform3D* Transform3D;
        public PlayerSys* Player;
    }

    public override void Update(Frame f, ref GunHolderFilter filter)
    {
    }

    public void SetPrimaryWeapon(Frame frame, WeaponData data, ref GunHolderFilter filter)
    {
        filter.Inventory->GetWeaponFromType(frame, WeaponType.Primary)->data = data;

        if (frame.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* player))
        {
            frame.Events.InstanceWeapon(player->Player, data);
        }
    }

    public void SetSecondaryWeapon(Frame frame, WeaponData data, ref GunHolderFilter filter)
    {
        filter.Inventory->GetWeaponFromType(frame, WeaponType.Secondary)->data = data;

        if (frame.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* player))
        {
            frame.Events.InstanceWeapon(player->Player, data);
        }
    }

    public void SetTertiaryWeapon(Frame frame, WeaponData data, ref GunHolderFilter filter)
    {
        filter.Inventory->GetWeaponFromType(frame, WeaponType.Tertiary)->data = data;

        if (frame.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* player))
        {
            frame.Events.InstanceWeapon(player->Player, data);
        }
    }
}