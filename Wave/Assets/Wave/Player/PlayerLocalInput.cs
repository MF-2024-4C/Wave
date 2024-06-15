using System;
using UnityEngine;
using Quantum;
using Photon.Deterministic;

namespace Wave.Player
{
    public class PlayerLocalInput : MonoBehaviour
    {
        private DispatcherSubscription _subscription;
        
        private void OnEnable()
        {
            _subscription = QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
        }

        private void OnDisable()
        {
            QuantumCallback.Unsubscribe(_subscription);
        }

        public void PollInput(CallbackPollInput callback)
        {
            Quantum.Input input = new Quantum.Input();
            input.PlayerJump = UnityEngine.Input.GetButton("Jump");
            input.PlayerDash = UnityEngine.Input.GetButton("Fire3");
            input.Interact = UnityEngine.Input.GetKey(KeyCode.F);
            var x = UnityEngine.Input.GetAxis("Horizontal");
            var y = UnityEngine.Input.GetAxis("Vertical");
            Vector2 dir = new Vector2(x, y);
        
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
    }
}
