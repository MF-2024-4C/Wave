using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ CreateAssetMenu( fileName = "NavigationMarkerData", menuName = "Wave/NavigationMarker/NavigationMarkerData" ) ]
public class NavigationMarkerData : ScriptableObject
{
    private Color Color;
    private Sprite Icon;
    private string Description;
}
