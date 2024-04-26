using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Quantum;
using UnityEngine;

namespace Wave.Player
{    
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private EntityView _entityView;
        [SerializeField] private GameObject _virtualCameraPrefab;

        public void OnEntityInstantiated()
        {
            QuantumGame game = QuantumRunner.Default.Game;
            Frame frame = game.Frames.Verified;

            if (frame.TryGet(_entityView.EntityRef, out PlayerLink playerLink))
            {
                if (!game.PlayerIsLocal(playerLink.Player)) return;

                GameObject virtualCameraObject = Instantiate(_virtualCameraPrefab);
                if (virtualCameraObject.TryGetComponent<CinemachineVirtualCamera>(
                        out CinemachineVirtualCamera virtualCamera))
                {
                    virtualCamera.m_Follow = this.transform;
                }
            }
        }
    }
}
