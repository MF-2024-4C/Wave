﻿namespace Quantum.Wave.Weapon;

public static unsafe class WeaponFireUtilities
{
    public static bool ProjectileCast(Frame frame, EntityRef causing, Transform3D* transform, PlayerSys* playerSys,
        Input* input)
    {
        var config = frame.FindAsset<PlayerConfig>(playerSys->Config.Id);
        var start = transform->Position + config.InteractRayOffset;
        var end = start + input->CameraForwardDirection * 50;
        var hits = frame.Physics3D.LinecastAll(start, end);
        for (var i = 0; i < hits.Count; i++)
        {
            var hit = hits[i];
            if (frame.Unsafe.TryGetPointer(hit.Entity, out Quantum.Zombie* _))
            {
                var damageSource = new DamageSource
                {
                    Causing = causing,
                    Target = hit.Entity,
                    Type = DamageType.Gun,
                };
                
                frame.Signals.OnDamage(hit.Entity, damageSource, 100);
                return true;
            }
        }

        return false;
    }
}