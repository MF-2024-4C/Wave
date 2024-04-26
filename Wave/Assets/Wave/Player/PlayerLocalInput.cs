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
            var x = UnityEngine.Input.GetAxis("Horizontal");
            var y = UnityEngine.Input.GetAxis("Vertical");
            Vector2 dir = new Vector2(x, y);
        
            //カメラの向きと入力を合わせる
            Vector3 cameraForward = Vector3.Scale(UnityEngine.Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 moveForward = cameraForward * dir.y + UnityEngine.Camera.main.transform.right * dir.x;
            dir.x = moveForward.x;
            dir.y = moveForward.z;
            input.PlayerDirection = dir.ToFPVector2();
        
            callback.SetInput(input, DeterministicInputFlags.Repeatable);
        }
    }
}
