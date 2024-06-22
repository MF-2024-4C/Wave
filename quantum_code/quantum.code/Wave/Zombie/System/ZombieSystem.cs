using Photon.Deterministic;

namespace Quantum.Wave.Zombie;

public unsafe class ZombieSystem : SystemMainThreadFilter<ZombieSystem.Filter>,ISignalOnDamage
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
        if (!frame.Map.NavMeshes.TryGetValue("Navmesh", out var mesh))
        {
            Log.Warn("MapにNavMeshが存在しません。");
            return;
        }
        _navMesh = mesh;
    }

    private const int UpdateTargetPosPerFrame = 20;

    public override void Update(Frame f, ref Filter filter)
    {
        if (filter.Zombie->State == ZombieState.Die)
        {
            return;
        }

        var spec = f.FindAsset<ZombieSpec>(filter.Zombie->Spec.Id);
        
        if (f.Exists(filter.Zombie->Target) == false)
        {
            if (filter.Zombie->State == ZombieState.Sleep || filter.Zombie->State == ZombieState.Idle)
            {
            
                foreach (var t in f.Unsafe.GetComponentBlockIterator<ZombieTargetable>())
                {
                    f.Unsafe.TryGetPointer<Transform3D>(t.Entity, out var transform);
                    var distance = FPMath.Abs((transform->Position - filter.Transform->Position).Magnitude);
                    if (distance < spec.DormantSearchDistance)
                    {
                        filter.Zombie->Target = t.Entity;
                        filter.Zombie->State = ZombieState.Chase;
                        break;
                    }
                }

                if (filter.Zombie->State == ZombieState.Sleep || filter.Zombie->State == ZombieState.Idle)
                {
                    return;
                }
            }
            
            if (filter.Zombie->SetMonsterTarget(f))
            {
                //filter.Zombie->State = ZombieState.Walk;
            }
        }

        if (f.Number % UpdateTargetPosPerFrame != 0)
        {
            return;
        }

        if (f.Exists(filter.Zombie->Target) == false) return;
        if (filter.Zombie->State != ZombieState.Chase) return;
        var targetTransform = f.Unsafe.GetPointer<Transform3D>(filter.Zombie->Target);

        if (f.Unsafe.TryGetPointer(filter.Entity, out NavMeshPathfinder* pathfinder))
        {
            pathfinder->SetTarget(f, targetTransform->Position, _navMesh);
        }
    }

    public void OnDamage(Frame f, EntityRef target, FP damage)
    {
        if (f.Unsafe.TryGetPointer(target, out Quantum.Zombie* zombie))
        {
            zombie->HP -= damage;
            if (zombie->HP <= 0)
            {
                zombie->State = ZombieState.Die;
            }
        }
    }
}