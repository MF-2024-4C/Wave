using System;
using UnityEngine;
using Quantum;
using Photon.Deterministic;
using Input = UnityEngine.Input;

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
            input.PlayerJump = Input.GetButton("Jump");
            input.PlayerDash = Input.GetButton("Fire3");
            var x = Input.GetAxis("Horizontal");
            var y = Input.GetAxis("Vertical");
            Vector2 dir = new Vector2(x, y);
            
            //Weapon
            input.ChangePrimaryWeapon = Input.GetKey(KeyCode.Alpha1);
            input.ChangeSecondaryWeapon = Input.GetKey(KeyCode.Alpha2);
            input.ChangeTertiaryWeapon = Input.GetKey(KeyCode.Alpha3);
            
            input.Fire = Input.GetMouseButton(0);
        
            //カメラの向きと入力を合わせる
            Transform mainCameraTran = Camera.main.transform;
            Vector3 cameraForward = Vector3.Scale(mainCameraTran.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 moveForward = cameraForward * dir.y + mainCameraTran.right * dir.x;
            dir.x = moveForward.x;
            dir.y = moveForward.z;
            input.PlayerDirection = dir.ToFPVector2();
            
            //カメラの前方向を保存
            input.CameraForwardDirection = mainCameraTran.forward.ToFPVector3();
        
            callback.SetInput(input, DeterministicInputFlags.Repeatable);
        }
    }
}
