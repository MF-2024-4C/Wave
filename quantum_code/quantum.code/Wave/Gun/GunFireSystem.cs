using System.Diagnostics;

namespace Quantum.Wave.Gun;

public unsafe struct PlayerFilter
{
    public EntityRef Entity;
    public PlayerSys* Player;
}

public unsafe class GunFireSystem : SystemMainThreadFilter<PlayerFilter>
{
    public override void Update(Frame frame, ref PlayerFilter filter)
    {
        Input input = default;
        if (!frame.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* player)) return;

        input = *frame.GetPlayerInput(player->Player);

        if (input.Fire.WasPressed)
        {
            Log.Info($"Player {filter.Entity.Index} fired! FrameNumber:{frame.Number}");
            frame.Events.Fire(player->Player);
        }
    }
}