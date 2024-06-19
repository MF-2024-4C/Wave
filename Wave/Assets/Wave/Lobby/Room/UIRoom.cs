using ExitGames.Client.Photon;
using Michsky.UI.Heat;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Wave.Lobby
{
    public class UIRoom : MonoBehaviour, IInRoomCallbacks
    {
        public static UIRoom Instance;

        [SerializeField] private ButtonManager _playButtonManager;
        [SerializeField] private TMP_InputField _roomNameInputField;
        [SerializeField] private SwitchManager _privateSwitchManager;

        [SerializeField] private OverlayCharacterManager _overlayCharacterManager;


        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void OnJoinRoom()
        {
            _playButtonManager.gameObject.SetActive(WaveUIConnect.Client.LocalPlayer.IsMasterClient);

            _roomNameInputField.interactable = WaveUIConnect.Client.LocalPlayer.IsMasterClient;
            _roomNameInputField.text = WaveUIConnect.Client.CurrentRoom.Name;

            _privateSwitchManager.isOn = WaveUIConnect.Client.CurrentRoom.IsVisible;
            _privateSwitchManager.isInteractable = WaveUIConnect.Client.LocalPlayer.IsMasterClient;

            foreach (var player in WaveUIConnect.Client.CurrentRoom.Players.Values)
            {
                _overlayCharacterManager.AddOverlayCharacter(player);
            }
        }


        public void OnRoomNameChanged()
        {
            //WaveUIConnect.Client.CurrentRoom.Name = _roomNameInputField.text;
        }

        public void OnPrivateSwitchChanged()
        {
            WaveUIConnect.Client.CurrentRoom.IsVisible = _privateSwitchManager.isOn;
        }

        public void LeaveRoom()
        {
            WaveUIConnect.Client.OpLeaveRoom(true);

            _overlayCharacterManager.AllRemoveOverlayCharacter();
        }

        #region IInRoomCallbacks

        public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            Debug.Log("新しいプレイヤーが入室しました");
            _overlayCharacterManager.AddOverlayCharacter(newPlayer);
        }

        public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            Debug.Log("プレイヤーが退室しました");
            _overlayCharacterManager.RemoveOverlayCharacter(otherPlayer);
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            //Todo: Update Room Name
            _roomNameInputField.text = WaveUIConnect.Client.CurrentRoom.Name;
            _privateSwitchManager.isOn = WaveUIConnect.Client.CurrentRoom.IsVisible;
        }

        public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
        }

        public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
        {
        }

        #endregion
    }
}