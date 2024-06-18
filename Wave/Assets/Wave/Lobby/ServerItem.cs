using Photon.Realtime;
using TMPro;
using UnityEngine;

public class ServerItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _roomName;

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        _roomName.text = roomInfo.Name;
    }
}