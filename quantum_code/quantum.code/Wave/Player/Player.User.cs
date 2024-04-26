using Quantum.QuantumDemo;
using Photon.Deterministic;

namespace Quantum
{
    public unsafe partial struct PlayerSys
    {
        public static void Move(Frame f, EntityRef entityRef, CharacterController3D* controller, PlayerSys* playerSys)
        {
            PlayerConfig config = f.FindAsset<PlayerConfig>(playerSys->Config.Id);
            CharacterController3DConfig cconfig = f.FindAsset<CharacterController3DConfig>(controller->Config.Id);

            cconfig.Braking = config.BreakPower;
            
            Input input = default;
            if (f.Unsafe.TryGetPointer(entityRef, out PlayerLink* playerLink))
            {
                input = *f.GetPlayerInput(playerLink->Player);
            }

            if (input.PlayerJump.WasPressed)
            {
                controller->Jump(f, false, config.JumpPower);
            }
            
            
            FP speed = config.WalkSpeed;
            if (input.PlayerDash) speed = config.RunSpeed;
            controller->MaxSpeed = speed;
            controller->Move(f, entityRef, input.PlayerDirection.XOY);
        }
        
        public static void Rot(Frame f, EntityRef entity, Transform3D* transform, CharacterController3D* controller, PlayerSys* playerSys)
        {
            PlayerConfig config = f.FindAsset<PlayerConfig>(playerSys->Config.Id);
            
            FPQuaternion targetRotation = FPQuaternion.LookRotation(controller->Velocity);
            targetRotation.X = 0;
            targetRotation.Z = 0;
            transform->Rotation = FPQuaternion.Slerp(transform->Rotation, targetRotation, f.DeltaTime * config.RotationSpeed);
        }
    }
}