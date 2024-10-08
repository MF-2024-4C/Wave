using UnityEngine;
using Quantum;
using UnityEngine.UI;
using LayerMask = UnityEngine.LayerMask;

public class PlayerInteractUI : MonoBehaviour
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
        //自分が倒れている場合はUIを表示しない
        var frame = QuantumRunner.Default.Game.Frames.Verified;
        if (!frame.TryGet<PlayerSys>(_entityView.EntityRef, out PlayerSys playerSys)) return false;
        if (playerSys.IsDead) return false;
        
        //インタラクトする向きと距離を取得
        var prototypeConfig = _entityComponentPlayerSys.Prototype.Config;
        if (prototypeConfig == null) return false;
        Vector3 from = FPMathUtils.ToUnityVector3(playerSys.InteractRayOffset) + transform.root.position;
        Vector3 forward = FPMathUtils.ToUnityVector3(playerSys.CameraForwardDirection);
        float distance = playerSys.InteractRayDistance.AsFloat;

        if (forward == Vector3.zero)
        {
            forward = transform.root.forward;
        }

        //Raycastで対象がいるかどうかをチェック
        var hits = Physics.RaycastAll(from, forward, distance);
        
        //Raycastで当たったEntityViewがInteractorを持っているかどうかをチェック
        foreach (RaycastHit hit in hits)
        {
            //自分だった場合は飛ばす
            if (hit.transform.root == transform.root) continue;
            
            hitEntityView = hit.transform.GetComponentInParent<EntityView>();
            if (hitEntityView == null) continue;
            
            if (!frame.TryGet<Interactor>(hitEntityView.EntityRef,out var hitInteractor)) continue;
            if (!hitInteractor.CanInteract || hitInteractor.OnInteract || hitInteractor.NowCoolDown) continue;
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
