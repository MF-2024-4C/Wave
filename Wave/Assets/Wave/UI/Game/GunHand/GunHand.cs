using System;
using UnityEngine;
using UnityEngine.Animations;

namespace Wave.UI.Game.GunHand
{
    [RequireComponent(typeof(AimConstraint))]
    public class GunHand : MonoBehaviour
    {
        private AimConstraint _aimConstraint;

        public void Active()
        {
            _aimConstraint = GetComponent<AimConstraint>();
            _aimConstraint.AddSource(new ConstraintSource
                { sourceTransform = Camera.main.transform.GetChild(0), weight = 1.0f });
        }
    }
}