namespace Quantum.Wave.Zombie;

public unsafe class ZombieSystem : SystemMainThreadFilter<ZombieSystem.Filter>
{
    public struct Filter
    {
        public EntityRef Entity;
        public Transform3D* Transform;
        public Quantum.Zombie* Zombie;
    }

    private NavMesh _navMesh;

    public override void OnInit(Frame frame)
    {
        _navMesh = frame.Map.NavMeshes["Navmesh"];
        Assert.Check(_navMesh != null, "MapにNavMeshが存在しません。");
    }

    private const int UpdateTargetPosPerFrame = 20;

    public override void Update(Frame f, ref Filter filter)
    {
        if (filter.Zombie->ShouldDie)
        {
            f.Destroy(filter.Entity);
            return;
        }

        if (f.Exists(filter.Zombie->Target) == false)
        {
            filter.Zombie->SetMonsterTarget(f);
        }

        if (f.Number % UpdateTargetPosPerFrame != 0)
        {
            return;
        }

        if (f.Exists(filter.Zombie->Target) == false) return;
        var targetTransform = f.Unsafe.GetPointer<Transform3D>(filter.Zombie->Target);

        if (f.Unsafe.TryGetPointer(filter.Entity, out NavMeshPathfinder* pathfinder))
        {
            pathfinder->SetTarget(f, targetTransform->Position, _navMesh);
        }
    }
}