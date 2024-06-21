using Quantum;
using UnityEngine;
using UnityEngine.Serialization;

namespace Wave.Lobby.MapManager
{
    [CreateAssetMenu(fileName = "MapInfo", menuName = "Wave/Lobby/MapManager/MapInfo")]
    public class MapInfo : ScriptableObject
    {
        public MapAsset MapAsset;
        
        public string MapName;
        public string MapDescription;
        public Sprite PreviewImage;
    }
}