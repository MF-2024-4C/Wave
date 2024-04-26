namespace Quantum.Player
{
    public unsafe class PlayerSystem : SystemMainThreadFilter<PlayerSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public Transform3D* Transform;
            public CharacterController3D* CharacterController;
            public PlayerSys* Player;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            Input input = default;
            if (f.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* playerLink))
            {
                input = *f.GetPlayerInput(playerLink->Player);
            }
            
            PlayerSys.Move(f, filter.Entity, filter.CharacterController, filter.Player, input);
            PlayerSys.Rot(f, filter.Entity, filter.Transform, filter.CharacterController, filter.Player, input);
        }
    }
}

