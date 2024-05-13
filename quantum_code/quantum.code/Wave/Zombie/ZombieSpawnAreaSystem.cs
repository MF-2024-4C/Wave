namespace Quantum;

public unsafe class ZombieSpawnAreaSystem : SystemMainThreadFilter<ZombieSpawnAreaSystem.Filter>
{
    public struct Filter
    {
        public EntityRef Entity;
        public ZombieSpawnArea* ZombieSpawnArea;
    }

    public override void Update(Frame f, ref Filter filter)
    {
        
    }
}