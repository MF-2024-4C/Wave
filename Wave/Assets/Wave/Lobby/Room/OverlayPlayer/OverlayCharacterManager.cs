using TMPro;
using UnityEngine;

public class OverlayCharacterManager : MonoBehaviour
{
    [SerializeField] private Transform[] _overlayCharacterParent = new Transform[4];
    [SerializeField] private TextMeshProUGUI[] _overlayCharacterText = new TextMeshProUGUI[4];
    [SerializeField] private OverlayCharacter _overlayCharacterPrefab;

    private void Awake()
    {
        foreach (var text in _overlayCharacterText)
        {
            text.text = "";
        }
    }

    public void AddOverlayCharacter(Photon.Realtime.Player player)
    {
        var overlayCharacter =
            Instantiate(_overlayCharacterPrefab, _overlayCharacterParent[player.ActorNumber - 1]);
        overlayCharacter.SetPlayer(player);
        _overlayCharacterText[player.ActorNumber - 1].text = player.NickName;
    }

    public void RemoveOverlayCharacter(Photon.Realtime.Player player)
    {
        foreach (Transform child in _overlayCharacterParent[player.ActorNumber - 1])
        {
            Destroy(child.gameObject);
        }

        _overlayCharacterText[player.ActorNumber - 1].text = "";
    }

    public void AllRemoveOverlayCharacter()
    {
        foreach (var child in _overlayCharacterParent)
        {
            foreach (Transform grandChild in child)
            {
                Destroy(grandChild.gameObject);
            }
        }

        foreach (var text in _overlayCharacterText)
        {
            text.text = "";
        }
    }
}