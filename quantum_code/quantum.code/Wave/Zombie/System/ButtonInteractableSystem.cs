using Photon.Deterministic;

namespace Quantum.Wave.Zombie;

public unsafe class ButtonInteractableSystem : SystemMainThread
{
    public override void Update(Frame f)
    {
        var iterator = f.Unsafe.GetComponentBlockIterator<Character>();
        Input input = default;
        foreach (var (entity, character) in iterator)
        {
            if (f.Unsafe.TryGetPointer(entity, out PlayerLink* playerLink))
            {
                input = *f.GetPlayerInput(playerLink->Player);
            }

            if (!input.Interact.WasPressed) continue;
            var transform = f.Unsafe.GetPointer<Transform3D>(entity);
            var hits = f.Physics3D.LinecastAll(transform->Position,transform->Position + transform->Forward * FP._2);
            for (var i = 0; i < hits.Count; i++)
            {
                var hit = hits[i];
                if (!f.Unsafe.TryGetPointer(hit.Entity, out ButtonEntity* interactable)) continue;
                f.Signals.ActivateEvent(interactable->trigger);
            }
        }
    }
}