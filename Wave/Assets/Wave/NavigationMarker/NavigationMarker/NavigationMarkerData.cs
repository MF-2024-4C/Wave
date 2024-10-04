using UnityEngine;

namespace Wave.NavigationMarker.NavigationMarker
{
    [ CreateAssetMenu( fileName = "NavigationMarkerData", menuName = "Wave/NavigationMarker/NavigationMarkerData" ) ]
    public class NavigationMarkerData : ScriptableObject
    {
        public Color Color = Color.white;
        public string MainText = "Head to your destination.";
        [Multiline(3)] public string SubText = "";
        public Sprite Icon;
    }
}
