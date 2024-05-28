using Quantum.Player;

namespace Quantum.Wave.Weapon;

public unsafe class WeaponInventorySystem : SystemMainThreadFilter<WeaponInventorySystem.GunHolderFilter>
{
    public struct GunHolderFilter
    {
        public EntityRef Entity;
        public WeaponInventory* Inventory;
    }
    
    public override void Update(Frame f, ref GunHolderFilter filter)
    {
    }

    public void SetPrimaryWeapon(Frame frame, WeaponData data, ref GunHolderFilter filter)
    {
        filter.Inventory->PrimaryWeaponData = data;
        
        if (frame.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* player))
        {
            frame.Events.InstanceWeapon(player->Player, data);
        }
    }

    public void SetSecondaryWeapon(Frame frame, WeaponData data, ref GunHolderFilter filter)
    {
        filter.Inventory->SecondaryWeaponData = data;
        
        if (frame.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* player))
        {
            frame.Events.InstanceWeapon(player->Player, data);
        }
    }

    public void SetTertiaryWeapon(Frame frame, WeaponData data, ref GunHolderFilter filter)
    {
        filter.Inventory->TertiaryWeaponData = data;
        
        if (frame.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* player))
        {
            frame.Events.InstanceWeapon(player->Player, data);
        }
    }
}