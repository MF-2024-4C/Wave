using System;
using UnityEngine;
using Quantum;
using UnityEngine.UI;
using LayerMask = UnityEngine.LayerMask;

public class PlayerInteractUI : MonoBehaviour
{
    [SerializeField] private EntityView _entityView;
    [SerializeField] private GameObject _canInteractUI;
    [SerializeField] private Image _elapsedTimeUI;
    
    private DispatcherSubscription _CheckCanInteractEventSubscript;
    private DispatcherSubscription _CheckElapsedTimeEventSubscript;
    private bool _canInteract;
    private bool _nowInteract;
    private float _elapsedTime;

    private void OnEnable()
    {
        _CheckCanInteractEventSubscript = QuantumEvent.Subscribe<EventPlayerCanInteractEvent>(this, OnCheckInteract);
        _CheckElapsedTimeEventSubscript = QuantumEvent.Subscribe<EventPlayerInteractElapseEvent>(this, OnCheckElapseTime);
    }
    
    private void OnDisable()
    {
        QuantumEvent.Unsubscribe(_CheckCanInteractEventSubscript);
        QuantumEvent.Unsubscribe(_CheckElapsedTimeEventSubscript);
    }

    private void Start()
    {
        QuantumGame game = QuantumRunner.Default.Game;
        Frame frame = game.Frames.Verified;
        _canInteract = false;
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
        if (_canInteract)
        {
            _canInteractUI.SetActive(true);
            return;
        }
        
        _canInteractUI.SetActive(false);
        
        /*
        if (!CheckElapsedTime(out float elapsedTimeRate))
        {
            _elapsedTimeUI.enabled = false;
            return;
        }
        
        _elapsedTimeUI.enabled = true;
        _elapsedTimeUI.fillAmount = elapsedTimeRate;
        */

        if (!_nowInteract)
        {
            _elapsedTimeUI.enabled = false;
            return;
        }

        _elapsedTimeUI.enabled = true;
        _elapsedTimeUI.fillAmount = _elapsedTime;
    }

    private void OnCheckInteract(EventPlayerCanInteractEvent e)
    {
        var frame = QuantumRunner.Default.Game.Frames.Predicted;
        if(e.PlayerLink.Player != frame.Get<PlayerLink>(_entityView.EntityRef).Player) return;
        _canInteract = e.IsCanInteract;
    }

    private void OnCheckElapseTime(EventPlayerInteractElapseEvent e)
    {
        var frame = QuantumRunner.Default.Game.Frames.Predicted;
        if(e.PlayerLink.Player != frame.Get<PlayerLink>(_entityView.EntityRef).Player) return;
        _nowInteract = e.IsInteract;
        _elapsedTime = e.ElapseTime.AsFloat;
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
