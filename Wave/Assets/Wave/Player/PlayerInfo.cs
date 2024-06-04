using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Deterministic;
using Quantum;
using TMPro.SpriteAssetUtilities;
using UnityEngine;

namespace Wave.Player
{
    public class PlayerInfo : MonoBehaviour
    {
        [SerializeField] private EntityComponentPlayerSys _playerSys;
        private void OnValidate()
        {
            _playerSys = GetComponent<EntityComponentPlayerSys>();
        }

        private void OnDrawGizmos()
        {
            
            //Interact Line Gizmos
            var playerSys = _playerSys.Prototype.Config;
            if (playerSys == null) return;
            var playerConfig = UnityDB.FindAsset<PlayerConfigAsset>(playerSys.Id).Settings;
            Vector3 from = FPMathUtils.ToUnityVector3(playerConfig.InteractRayOffset) + transform.position;
            Vector3 forward = FPMathUtils.ToUnityVector3(playerConfig.CameraForwardDirection);
            float distance = playerConfig.InteractRayDistance.AsFloat;
            //Debug.Log($"Config");

            if (forward == Vector3.zero)
            {
                forward = transform.forward;
                Debug.Log("camera forward is zero");
            }
            
            Gizmos.DrawLine(from, from + (forward * distance));
        }
    }
}
