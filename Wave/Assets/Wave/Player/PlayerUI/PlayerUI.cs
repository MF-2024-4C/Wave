using UnityEngine;
using Quantum;
using LayerMask = UnityEngine.LayerMask;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private EntityView _entityView;
    [SerializeField] private EntityComponentPlayerSys _entityComponentPlayerSys;
    [SerializeField] private LayerMask _interactLayer;
    [SerializeField] private GameObject _canInteractUI;

    private void Start()
    {
        QuantumGame game = QuantumRunner.Default.Game;
        Frame frame = game.Frames.Verified;
        if(frame.TryGet(_entityView.EntityRef, out PlayerLink playerLink))
        {
            if (!game.PlayerIsLocal(playerLink.Player))
            {
                this.gameObject.SetActive(false);
            }
        }
    }
    
    private void Update()
    {
        _canInteractUI.SetActive(CheckCanInteract());
    }
    
    private bool CheckCanInteract()
    {
        EntityView hitEntityView = null;
        var frame = QuantumRunner.Default.Game.Frames.Verified;
        if (!frame.TryGet<PlayerSys>(_entityView.EntityRef, out PlayerSys playerSys)) return false;
        
        var prototypeConfig = _entityComponentPlayerSys.Prototype.Config;
        if (prototypeConfig == null) return false;
        var playerConfig = UnityDB.FindAsset<PlayerConfigAsset>(prototypeConfig.Id).Settings;
        Vector3 from = FPMathUtils.ToUnityVector3(playerConfig.InteractRayOffset) + transform.root.position;
        Vector3 forward = FPMathUtils.ToUnityVector3(playerSys.CameraForwardDirection);
        float distance = playerConfig.InteractRayDistance.AsFloat;

        if (forward == Vector3.zero)
        {
            forward = transform.root.forward;
        }

        var hits = Physics.RaycastAll(from, forward, distance, _interactLayer);
        
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
