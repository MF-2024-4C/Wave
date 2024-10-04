namespace Quantum.Wave.Weapon;

public class WeaponSetupSystem : SystemSignalsOnly, ISignalOnComponentAdded<Quantum.Weapon>
{
    public unsafe void OnAdded(Frame frame, EntityRef entity, Quantum.Weapon* component)
    {
        var weaponData = frame.FindAsset<WeaponData>(component->data.Id);
        weaponData.Initialize(component);
    }
}