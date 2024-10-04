using UnityEngine;

namespace Wave.Lobby.MapManager
{
    [CreateAssetMenu(fileName = "MapManager", menuName = "Wave/Lobby/MapManager/MapManager")]
    public class MapManager : ScriptableObject
    {
        public MapInfo[] Maps;
    }

}
