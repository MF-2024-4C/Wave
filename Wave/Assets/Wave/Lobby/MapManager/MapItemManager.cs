using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wave.Lobby.Room;

namespace Wave.Lobby.MapManager
{
    public class MapItemManager : MonoBehaviour
    {
        [HideInInspector] public MapInfo _mapInfo;
        [SerializeField] private TextMeshProUGUI _mapName;
        [SerializeField] private TextMeshProUGUI _mapDescription;
        [SerializeField] private Image _previewImage;

        public void SetMapInfo(MapInfo mapInfo)
        {
            _mapInfo = mapInfo;
            _mapName.text = _mapInfo.MapName;
            _mapDescription.text = _mapInfo.MapDescription;
            _previewImage.sprite = _mapInfo.PreviewImage;
        }

        public void OnClick()
        {
            UIRoom.Instance.SetMap(_mapInfo);
        }
    }
}