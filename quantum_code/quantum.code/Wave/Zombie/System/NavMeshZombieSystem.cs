using Photon.Deterministic;

namespace Quantum.Wave.Zombie;

public unsafe class NavMeshMovementZombieSystem : SystemSignalsOnly,ISignalOnNavMeshWaypointReached,ISignalOnNavMeshSearchFailed,ISignalOnNavMeshMoveAgent
{
    public void OnNavMeshWaypointReached(Frame f, EntityRef entity, FPVector3 waypoint, Navigation.WaypointFlag waypointFlags,
        ref bool resetAgent)
    {
        
    }

    public void OnNavMeshSearchFailed(Frame f, EntityRef entity, ref bool resetAgent)
    {
    }

    public void OnNavMeshMoveAgent(Frame f, EntityRef entity, FPVector2 desiredDirection)
    {
        var pathfinder = f.Unsafe.GetPointer<NavMeshPathfinder>(entity);
        var transform = f.Unsafe.GetPointer<Transform3D>(entity);

        var currentWayPointIndex = pathfinder->WaypointIndex;
        var wayPoint = pathfinder->GetWaypoint(f, currentWayPointIndex);

        var moveDirection = (wayPoint - transform->Position).Normalized;
        transform->Position += moveDirection * 2 * f.DeltaTime; 
        
        transform->Rotation = FPQuaternion.LookRotation(desiredDirection.XOY, FPVector3.Up);
    }
}