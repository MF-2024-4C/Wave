namespace Quantum.Player
{
    public unsafe class PlayerSystem : SystemMainThreadFilter<PlayerSystem.PlayerFilter>
    {
        public struct PlayerFilter
        {
            public EntityRef Entity;
            public Transform3D* Transform;
            public CharacterController3D* CharacterController;
            public PlayerSys* Player;
        }

        public override void Update(Frame f, ref PlayerFilter playerFilter)
        {
            Input input = default;
            //PlayerAnimInfo* playerAnimInfo = f.Unsafe.GetPointer<PlayerAnimInfo>(filter.Entity);
            if (f.Unsafe.TryGetPointer(playerFilter.Entity, out PlayerLink* playerLink))
            {
                input = *f.GetPlayerInput(playerLink->Player);
            }

            f.Unsafe.TryGetPointer(playerFilter.Entity, out PlayerSys* playerLocalInfo);
            PlayerSys.Rot(f, playerFilter.Entity, playerFilter.Transform, playerFilter.CharacterController, playerFilter.Player, playerLocalInfo, input);
            PlayerSys.Move(f, playerFilter.Entity, playerFilter.CharacterController, playerFilter.Player, input, playerLocalInfo);
        }
    }
}

