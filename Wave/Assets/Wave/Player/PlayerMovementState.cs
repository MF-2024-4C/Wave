using Photon.Deterministic;
using Quantum;
using UnityEngine;

namespace Wave.Player
{
    public class PlayerMovementState : MonoBehaviour
    {
        public enum MovementState
        {
            Idle,
            Walking,
            Sprinting,
            InAir
        }

        private EntityView _entityView;
        private QuantumGame _game;

        private string _currentTriggerAnim;

        public MovementState CurrentMovementState { get; private set; }
        public Vector2 AnimatorVelocity { get; private set; }

        private Animator _animator;

        #region AnimatorParams

        private static readonly int InAir = Animator.StringToHash("InAir");
        private static readonly int MoveX = Animator.StringToHash("MoveX");
        private static readonly int MoveY = Animator.StringToHash("MoveY");
        private static readonly int Velocity = Animator.StringToHash("Velocity");
        private static readonly int Moving = Animator.StringToHash("Moving");
        private static readonly int Sprinting = Animator.StringToHash("Sprinting");

        #endregion

        private void Awake()
        {
            _currentTriggerAnim = "idle";
            _entityView = GetComponent<EntityView>();
            _entityView.OnEntityInstantiated.AddListener(OnEntityInstantiated);
        }

        private void OnEntityInstantiated(QuantumGame game)
        {
            _game = game;
            _animator = GetComponentInChildren<Animator>();
            
            QuantumEvent.Subscribe<EventFire>(this, OnFire);
        }
        
        private void OnFire(EventFire e)
        {
            
        }

        public void Update()
        {
            var frame = _game.Frames.Verified;
            if (!frame.TryGet(_entityView.EntityRef, out PlayerSys playerLocalInfo)) return;
            var state = playerLocalInfo.PlayerAnimState;

            if ((state & PlayerConfig.PAnimMove) == PlayerConfig.PAnimMove)
            {
                if ((state & PlayerConfig.PAnimRun) == PlayerConfig.PAnimRun)
                {
                    CurrentMovementState = MovementState.Sprinting;
                }
                else
                {
                    CurrentMovementState = MovementState.Walking;
                }
            }
            else
            {
                CurrentMovementState = MovementState.Idle;
            }

            UpdateAnimatorParams(frame);
            UpdateRotation(playerLocalInfo);
        }

        private void UpdateAnimatorParams(Frame frame)
        {
            if (_animator == null) return;

            if (frame.TryGet(_entityView.EntityRef, out CharacterController3D characterController))
            {
                return;
            }

            var animatorVelocity = characterController.Velocity.ToUnityVector3();
            animatorVelocity *= CurrentMovementState == MovementState.InAir ? 0f : 1f;

            AnimatorVelocity = Vector2.Lerp(AnimatorVelocity, animatorVelocity,
                1 - Mathf.Exp(-5f * Time.deltaTime));

            _animator.SetFloat(MoveX, AnimatorVelocity.x);
            _animator.SetFloat(MoveY, AnimatorVelocity.y);
            _animator.SetFloat(Velocity, AnimatorVelocity.magnitude);
            _animator.SetBool(InAir, !characterController.Grounded);
            _animator.SetBool(Moving, !Mathf.Approximately(animatorVelocity.normalized.magnitude, 0f));
        }

        private void UpdateRotation(PlayerSys playerLocalInfo)
        {
            var targetRotation = playerLocalInfo.TargetRotation;
            this.transform.rotation = Quaternion.Slerp(transform.rotation,
                playerLocalInfo.TargetRotation.ToUnityQuaternion(), 10f);
        }
    }
}