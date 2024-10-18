using System;
using KINEMATION.FPSAnimationFramework.Runtime.Core;
using KINEMATION.KAnimationCore.Runtime.Input;
using Photon.Deterministic;
using Quantum;
using UnityEngine;
using UnityEngine.InputSystem;

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

        private UserInputController _userInput;
        private Vector2 _lookDeltaInput;
        private Vector2 _playerInput;
        private int _sensitivityMultiplierPropertyIndex;

        private WeaponInventory _weaponInventory;


        #region AnimatorParams

        private static readonly int InAir = Animator.StringToHash("InAir");
        private static readonly int MoveX = Animator.StringToHash("MoveX");
        private static readonly int MoveY = Animator.StringToHash("MoveY");
        private static readonly int Velocity = Animator.StringToHash("Velocity");
        private static readonly int Moving = Animator.StringToHash("Moving");
        private static readonly int Sprinting = Animator.StringToHash("Sprinting");
        private static readonly int ProneWeightHash = Animator.StringToHash("ProneWeight");

        #endregion

        private void Awake()
        {
            _currentTriggerAnim = "idle";
            _entityView = GetComponent<EntityView>();
            _userInput = GetComponent<UserInputController>();
            _weaponInventory = GetComponentInChildren<WeaponInventory>();
            _entityView.OnEntityInstantiated.AddListener(OnEntityInstantiated);
        }

        private void Start()
        {
            _sensitivityMultiplierPropertyIndex = _userInput.GetPropertyIndex("SensitivityMultiplier");
        }
        
        private void OnEntityInstantiated(QuantumGame game)
        {
            _game = game;
            _animator = GetComponentInChildren<Animator>();

            QuantumEvent.Subscribe<EventFire>(this, OnFired);
        }

        private void OnFired(EventFire e)
        {
            if (e.Owner != _entityView.EntityRef) return;
            _weaponInventory.CurrentWeapon.WeaponView.OnFire();
        }

        private void OnFireReleased()
        {
            _weaponInventory.CurrentWeapon.WeaponView.OnFireReleased();
        }

        private unsafe bool WasFireReleased()
        {
            var frame = _game.Frames.Verified;
            if (!frame.TryGet<PlayerLink>(_entityView.EntityRef, out var playerLink))
                return false;
            var input = frame.GetPlayerInput(playerLink.Player);
            return input->Fire.WasReleased;
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

            UpdateLookAt();
            UpdateAnimatorParams(frame);
            UpdateRotation(playerLocalInfo);

            if (WasFireReleased())
            {
                OnFireReleased();
            }
        }

        public void OnLook(InputValue value)
        {
            _lookDeltaInput = value.Get<Vector2>();
        }

        private void UpdateLookAt()
        {
            float scale = _userInput.GetValue<float>(_sensitivityMultiplierPropertyIndex);

            float deltaMouseX = _lookDeltaInput.x * 1f * scale;
            float deltaMouseY = -_lookDeltaInput.y * 1f * scale;

            _playerInput.y += deltaMouseY;
            _playerInput.x += deltaMouseX;

            float proneWeight = _animator.GetFloat(ProneWeightHash);
            Vector2 pitchClamp = Vector2.Lerp(new Vector2(-90f, 90f), new Vector2(-30, 0f), proneWeight);

            _playerInput.y = Mathf.Clamp(_playerInput.y, pitchClamp.x, pitchClamp.y);

            transform.rotation *= Quaternion.Euler(0f, deltaMouseX, 0f);

            _userInput.SetValue(FPSANames.MouseDeltaInput, new Vector4(deltaMouseX, deltaMouseY));
            _userInput.SetValue(FPSANames.MouseInput, new Vector4(_playerInput.x, _playerInput.y));
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