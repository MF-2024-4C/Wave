namespace Quantum.Wave.Gun;

public unsafe struct PlayerFilter
{
    public EntityRef Entity;
    public TempPlayer* Player;
}

public unsafe class GunFireSystem : SystemMainThreadFilter<PlayerFilter>
{
    
    
    public override void Update(Frame f, ref PlayerFilter filter)
    {
    }
    
}