using Photon.Deterministic;

namespace Quantum
{
    partial class PlayerConfig
    {
        public const byte PAnimIdle = 0b00000000;
        public const byte PAnimMove = 0b00000001;
        public const byte PAnimRun  = 0b00000010;
        public const byte PAnimJump = 0b00000100;
        public const byte PAnimFall = 0b00001000;
        public const byte PAnimGrounded = 0b00010000;
        
        public FP WalkSpeed;
        public FP RunSpeed;
        public FP JumpPower;
        public FP BreakPower;
        
        public FP RotationSpeed;

        public FPVector3 InteractRayOffset;
        public FPVector3 CameraForwardDirection;
        public FP InteractRayDistance;
        public LayerMask InteractLayer;
    }
}

