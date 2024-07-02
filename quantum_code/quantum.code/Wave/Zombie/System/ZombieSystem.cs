using Photon.Deterministic;

namespace Quantum.Wave.Zombie;

public unsafe class ZombieSystem : SystemMainThreadFilter<ZombieSystem.Filter>,ISignalOnComponentAdded<Quantum.Zombie>,ISignalOnDamage
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

        if (filter.Zombie->State != ZombieState.Chase) return;
        if (f.Exists(filter.Zombie->Target) == false) return;

        var targetTransform = f.Get<Transform3D>(filter.Zombie->Target);

        var targetDistance = FPMath.Abs((targetTransform.Position - filter.Transform->Position).Magnitude);
        if (targetDistance < spec.AttackRange)
        {
            filter.Zombie->AttackIntervalTimer -= f.DeltaTime;
            if (filter.Zombie->AttackIntervalTimer <= 0)
            {
                filter.Zombie->AttackIntervalTimer = filter.Zombie->AttackInterval;
                f.Signals.OnDecHealth(filter.Zombie->Target,3);
            }
            
        }
        
        if (f.Number % UpdateTargetPosPerFrame != 0)
        {
            return;
        }


        if (f.Unsafe.TryGetPointer(filter.Entity, out NavMeshPathfinder* pathfinder))
        {
            pathfinder->SetTarget(f, targetTransform.Position, _navMesh);
        }

    }

    public void OnDamage(Frame f, EntityRef target, DamageSource source, FP amount)
    {
        if (!f.Unsafe.TryGetPointer(target, out Quantum.Zombie* zombie)) return;
        
        zombie->HP -= amount;
        if (zombie->HP <= 0)
        {
            zombie->State = ZombieState.Die;
            f.Remove<PhysicsCollider3D>(target);
            f.Remove<PhysicsBody3D>(target);
            f.Remove<NavMeshPathfinder>(target);
        }
    }

    public void OnAdded(Frame f, EntityRef entity, Quantum.Zombie* component)
    {
        component->AttackIntervalTimer = component->AttackInterval;
    }
}