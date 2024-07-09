using System;
using UnityEngine;
using Quantum;
using Photon.Deterministic;
using UnityEngine.InputSystem;

namespace Wave.Player
{
    public class PlayerLocalInput : MonoBehaviour
    {
        [SerializeField] private PlayerInput _playerInput;
        
        private DispatcherSubscription _subscription;

        private Vector2 _moveInput;
        private bool _isPushJump;
        private bool _isPushInteract;
        private bool _isDash;
        
        private bool _isInputSystem;
        
        private void OnEnable()
        {
            _subscription = QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));

            _isInputSystem = _playerInput != null;
        }

        private void OnDisable()
        {
            QuantumCallback.Unsubscribe(_subscription);

            _isInputSystem = false;
        }

        public void PollInput(CallbackPollInput callback)
        {
            Quantum.Input input = new Quantum.Input();
            if (!_isInputSystem) SetInputManager();

            input.PlayerJump = _isPushJump;
            input.PlayerDash = _isDash;
            input.Interact = _isPushInteract;
            _isPushJump = false;
            Vector2 dir = _moveInput;
            
            //カメラの向きと入力を合わせる
            Transform mainCameraTran = UnityEngine.Camera.main.transform;
            Vector3 cameraForward = Vector3.Scale(mainCameraTran.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 moveForward = cameraForward * dir.y + mainCameraTran.right * dir.x;
            dir.x = moveForward.x;
            dir.y = moveForward.z;
            input.PlayerDirection = dir.ToFPVector2();
            
            //カメラの前方向を保存
            input.CameraForwardDirection = mainCameraTran.forward.ToFPVector3();
        
            input.ChangePrimaryWeapon = UnityEngine.Input.GetKey(KeyCode.Alpha1);
            input.ChangeSecondaryWeapon = UnityEngine.Input.GetKey(KeyCode.Alpha2);
            input.ChangeTertiaryWeapon = UnityEngine.Input.GetKey(KeyCode.Alpha3);
            input.Fire = UnityEngine.Input.GetMouseButton(0);
            input.Reload = UnityEngine.Input.GetKey(KeyCode.R);
            
            callback.SetInput(input, DeterministicInputFlags.Repeatable);
        }

        public void OnMoveInput(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            _moveInput = value;
        }

        public void OnJumpInput(InputAction.CallbackContext context)
        {
            if (context.started) _isPushJump = true;
            else if(context.canceled) _isPushJump = false;
        }
        
        public void OnInteractInput(InputAction.CallbackContext context)
        {
            if (context.started) _isPushInteract = true;
            else if (context.canceled) _isPushInteract = false;
        }
        
        public void OnDashInput(InputAction.CallbackContext context)
        {
            if (context.started) _isDash = true;
            else if (context.canceled) _isDash = false;
        }

        private void SetInputManager()
        {
            _isPushJump = UnityEngine.Input.GetButton("Jump");
            _isDash = UnityEngine.Input.GetButton("Fire3");
            _isPushInteract = UnityEngine.Input.GetKey(KeyCode.F);
            var x = UnityEngine.Input.GetAxis("Horizontal");
            var y = UnityEngine.Input.GetAxis("Vertical");
            _moveInput = new Vector2(x, y);
        }
    }
}
