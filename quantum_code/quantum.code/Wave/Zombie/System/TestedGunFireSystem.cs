using Photon.Deterministic;

namespace Quantum.Wave.Zombie;


public unsafe struct PlayerFilter
{
    public EntityRef Entity;
    public Transform3D* Transform;
    public PlayerSys* Player;
}

public unsafe class TestedGunFireSystem : SystemMainThreadFilter<PlayerFilter>
{
    public override void Update(Frame frame, ref PlayerFilter filter)
    {
        Input input = default;
        if (!frame.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* player)) return;

        input = *frame.GetPlayerInput(player->Player);

        if (input.Fire.WasPressed)
        {
            var hit = frame.Physics3D.Raycast(filter.Transform->Position + FPVector3.Up, filter.Transform->Forward, FP._100);
            if (hit == null) return;
            Log.Info($"{filter.Transform->Position + FPVector3.Up},{filter.Transform->Forward}");
            Log.Info(hit.Value.Entity);
        }
    }
}