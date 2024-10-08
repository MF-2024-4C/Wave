using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace Wave.Player
{
    public class PlayerCameraUpdater : MonoBehaviour
    {
        [SerializeField] private PlayerCameraSO _playerCameraSO;
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        
        private void OnEnable()
        {
            _playerCameraSO.OnChangeCameraHeight += OnChangeCameraHeight;
        }
        
        private void OnDisable()
        {
            _playerCameraSO.OnChangeCameraHeight -= OnChangeCameraHeight;
        }
        
        private void OnChangeCameraHeight(float height)
        {
            //_virtualCameraのFollowOffsetのYをheightに変更する
            CinemachineTransposer transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            Vector3 followOffset = transposer.m_FollowOffset;
            followOffset.y = height;
            transposer.m_FollowOffset = followOffset;
        }
    }
}
