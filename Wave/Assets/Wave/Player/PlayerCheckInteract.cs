using System.Collections;
using System.Collections.Generic;
using Quantum;
using Quantum.QuantumDemo;
using TMPro;
using UnityEngine;
using LayerMask = UnityEngine.LayerMask;

//ここは後から別のスクリプトになりそう
public class PlayerCheckInteract : MonoBehaviour
{
    [SerializeField] private EntityComponentPlayerSys _playerSys;
    [SerializeField] private PlayerUISO playerUISO;
    [SerializeField] private LayerMask _interactLayer;

    private void Update()
    {
        if (CheckCanInteract()) playerUISO.canInteract = true;
        else playerUISO.canInteract = false;
    }

    private bool CheckCanInteract()
    {
        var playerSys = _playerSys.Prototype.Config;
        if (playerSys == null) return false;
        var playerConfig = UnityDB.FindAsset<PlayerConfigAsset>(playerSys.Id).Settings;
        Vector3 from = FPMathUtils.ToUnityVector3(playerConfig.InteractRayOffset) + transform.position;
        Vector3 forward = FPMathUtils.ToUnityVector3(playerConfig.CameraForwardDirection);
        float distance = playerConfig.InteractRayDistance.AsFloat;

        if (forward == Vector3.zero)
        {
            forward = transform.forward;
        }

        var hits = Physics.RaycastAll(from, forward, distance);

        EntityView hitEntityView = null;
        var frame = QuantumRunner.Default.Game.Frames.Verified;

        foreach (RaycastHit hit in hits)
        {
            Debug.Log("Hit Any");
            hitEntityView = null;
            hitEntityView = hit.transform.GetComponentInParent<EntityView>();
            if (hitEntityView == null) continue;
            if (!frame.TryGet<Interactor>(hitEntityView.EntityRef,out var hitInteractor)) continue;
            if (!hitInteractor.IsInteract) continue;
            Debug.Log("Hit Interacter!");
            return true;
        }

        Debug.Log("Not Hit Interacter!");
        return false;
    }
}
