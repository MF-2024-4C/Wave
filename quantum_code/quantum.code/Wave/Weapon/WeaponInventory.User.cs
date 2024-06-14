namespace Quantum;

public unsafe partial struct WeaponInventory
{
    public void SetWeapon(Frame frame, WeaponType type, EntityRef entityRef)
    {
        switch (type)
        {
            case WeaponType.Primary:
                primary = entityRef;
                break;
            case WeaponType.Secondary:
                secondary = entityRef;
                break;
            case WeaponType.Tertiary:
                tertiary = entityRef;
                break;
        }
    }

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

    public EntityRef GetWeaponEntityFromType(WeaponType type)
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
                return default;
        }

        return entity;
    }
    
    public EntityRef GetCurrentWeaponEntity()
    {
        return GetWeaponEntityFromType(currentWeaponType);
    }
    
    public Weapon* GetCurrentWeapon(Frame frame)
    {
        return GetWeaponFromType(frame, currentWeaponType);
    }
}