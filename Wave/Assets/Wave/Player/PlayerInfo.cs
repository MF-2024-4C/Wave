using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Deterministic;
using Quantum;
using TMPro.SpriteAssetUtilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Wave.Player
{
    public class PlayerInfo : MonoBehaviour
    {
        [SerializeField] private EntityView _entityView;
        [SerializeField] private EntityComponentPlayerSys _entityComponentPlayerSys;
        private void OnValidate()
        {
            _entityComponentPlayerSys = GetComponent<EntityComponentPlayerSys>();
        }

        private void OnDrawGizmos()
        {
            //Interact Line Gizmos

            #if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlaying) return;
            #endif
            QuantumGame game = QuantumRunner.Default.Game;
            Frame frame = game.Frames.Verified;
            frame.TryGet<PlayerSys>(_entityView.EntityRef, out PlayerSys playerSys);
            
            var assetRefPlayerConfig = _entityComponentPlayerSys.Prototype.Config;
            if (assetRefPlayerConfig == null) return;
            var playerConfig = UnityDB.FindAsset<PlayerConfigAsset>(assetRefPlayerConfig.Id).Settings;
            Vector3 from = FPMathUtils.ToUnityVector3(playerConfig.InteractRayOffset) + transform.position;
            Vector3 forward = FPMathUtils.ToUnityVector3(playerSys.CameraForwardDirection);
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
