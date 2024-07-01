namespace Quantum.Player
{
    public unsafe class PlayerSystem : SystemMainThreadFilter<PlayerSystem.PlayerFilter> , ISignalOnComponentAdded<PlayerSys>, ISignalOnDead
    {
        public struct PlayerFilter
        {
            public EntityRef Entity;
            public Transform3D* Transform;
            public CharacterController3D* CharacterController;
            public PlayerSys* Player;
        }

        public override void Update(Frame f, ref PlayerFilter filter)
        {
            Input input = default;
            //PlayerAnimInfo* playerAnimInfo = f.Unsafe.GetPointer<PlayerAnimInfo>(filter.Entity);
            if (f.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* playerLink))
            {
                input = *f.GetPlayerInput(playerLink->Player);
            }

            PlayerSys.Rot(f, filter.Entity, filter.Transform, filter.CharacterController, filter.Player, input);
            PlayerSys.Move(f, filter.Entity, filter.CharacterController, filter.Player, input);
            PlayerSys.Interact(f, filter.Entity, filter.Transform, filter.Player, input);
        }

        public void OnAdded(Frame f, EntityRef entity, PlayerSys* component)
        {
            component->SetConfig(f, entity);
        }

        public void OnDead(Frame f, EntityRef entity)
        {
            if(!f.Unsafe.TryGetPointer(entity, out PlayerSys* playerSys))
            {
                Log.Info("PlayerSys not found");
                return;
            }

            PlayerSys.Dead(f, entity, playerSys);
        }
    }
}

