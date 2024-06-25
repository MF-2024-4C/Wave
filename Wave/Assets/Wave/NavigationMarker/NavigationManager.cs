using System;
using System.Collections.Generic;
using UnityEngine;

namespace Wave.NavimationMarker
{
    public class NavigationManager : MonoBehaviour
    {
        [Serializable]
        public class NavigationPoint
        {
            [SerializeField] private Transform transform;
            public NavigationMarkerData markerData;
            public Vector3 Position => transform.position;
        }

        [SerializeField] public List<NavigationPoint> _navigationPoints;
    }
}