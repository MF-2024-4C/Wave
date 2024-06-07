namespace Quantum;

public unsafe partial struct WeaponInventory
{
    public Weapon* GetWeaponFromType(Frame frame, WeaponType type)
    {
        EntityRef entity;
        switch (type)
        {
            case WeaponType.Primary:
                entity = primary;
                break;
            case WeaponType.Secondary:
                entity = secondary;
                break;
            case WeaponType.Tertiary:
                entity = tertiary;
                break;
            default:
                return null;
        }

        if (entity == default) return null;

        var weapon = frame.Unsafe.GetPointer<Weapon>(entity);
        return weapon;
    }

    public Weapon* GetCurrentWeapon(Frame frame)
    {
        return GetWeaponFromType(frame, currentWeaponType);
    }
}