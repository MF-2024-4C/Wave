using UnityEngine;
using Quantum;
using UnityEngine.UI;
using LayerMask = UnityEngine.LayerMask;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private EntityView _entityView;
    [SerializeField] private EntityComponentPlayerSys _entityComponentPlayerSys;
    [SerializeField] private LayerMask _interactLayer;
    //[SerializeField] private GameObject _canInteractUI;
    [SerializeField] private GameObject _canInteractUI;
    [SerializeField] private Image _elapsedTimeUI;
    
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
        if (CheckCanInteract())
        {
            _canInteractUI.SetActive(true);
            return;
        }
        
        _canInteractUI.SetActive(false);
        
        if (!CheckElapsedTime(out float elapsedTimeRate))
        {
            _elapsedTimeUI.enabled = false;
            return;
        }
        
        _elapsedTimeUI.enabled = true;
        _elapsedTimeUI.fillAmount = elapsedTimeRate;
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
            //Debug.Log("Hit Any");
            hitEntityView = null;
            hitEntityView = hit.transform.GetComponentInParent<EntityView>();
            if (hitEntityView == null) continue;
            if (!frame.TryGet<Interactor>(hitEntityView.EntityRef,out var hitInteractor)) continue;
            Debug.Log($"Can Interact is {hitInteractor.CanInteract} :: OnInteract is {hitInteractor.OnInteract} :: NowCoolDown is {hitInteractor.NowCoolDown}");
            if (!hitInteractor.CanInteract || hitInteractor.OnInteract || hitInteractor.NowCoolDown) continue;
            //Debug.Log("Hit Interacter!");
            return true;
        }

        //Debug.Log("Not Hit Interacter!");
        return false;
    }

    private bool CheckElapsedTime(out float elapsedTimeRate)
    {
        elapsedTimeRate = 0;
        Frame frame = QuantumRunner.Default.Game.Frames.Verified;
        if (!frame.TryGet<PlayerSys>(_entityView.EntityRef, out PlayerSys playerSys)) return false;
        if (!frame.TryGet<Interactor>(playerSys.InteractEntity, out Interactor interactor)) return false;
        if (interactor.InteractPlayer != _entityView.EntityRef) return false;
        if(!frame.TryFindAsset<InteractConfig>(interactor.Config.Id, out InteractConfig interactConfig)) return false;
        
        float nowElapsedTime = (frame.Number * frame.DeltaTime - interactor.InteractStartTime).AsFloat;
        elapsedTimeRate = nowElapsedTime / interactConfig.HoldTime.AsFloat;
        elapsedTimeRate = Mathf.Clamp01(elapsedTimeRate);
        return true;
    }
}
