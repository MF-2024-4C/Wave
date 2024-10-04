using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Wave.Lobby.Room.OverlayPlayer
{
    public class OverlayCharacterManager : MonoBehaviour
    {
        [SerializeField] private Transform[] _overlayCharacterParent = new Transform[4];
        private List<OverlayCharacter> _overlayCharacters = new();
        [SerializeField] private OverlayCharacter _overlayCharacterPrefab;

        public void AddOverlayCharacter(Photon.Realtime.Player player)
        {
            var overlayCharacter = Instantiate(_overlayCharacterPrefab, _overlayCharacterParent[_overlayCharacters.Count]);

            var playerName = GeneratePlayerName(player);
            overlayCharacter.SetPlayer(player, playerName);

            _overlayCharacters.Add(overlayCharacter);
        }

        private string GeneratePlayerName(Photon.Realtime.Player player)
        {
            return player.NickName + (player.IsLocal ? "(Me)" : "");
        }

        public void ViewOverlayCharacter(List<Photon.Realtime.Player> players)
        {
            //players = SortPlayers(players);

            //Playerを追加
            foreach (var player in players)
            {
                AddOverlayCharacter(player);
            }
        }

        private List<Photon.Realtime.Player> SortPlayers(List<Photon.Realtime.Player> players)
        {
            //players.ActorNumberを元に並び替え
            players = players.OrderBy(player => player.ActorNumber).ToList();

            //ClientMasterを先頭に持ってくる
            var masterClient = players.FirstOrDefault(player => player.IsMasterClient);
            players.Remove(masterClient);
            players.Insert(0, masterClient);

            return players;
        }

        public void RemoveOverlayCharacter(Photon.Realtime.Player player)
        {
            //Playerを削除
            RemovePlayer(player);

            //_overlayCharactersを_overlayCharacters.Player.ActorNumberで並び替え
            //_overlayCharacters = SortOverlayCharacters(_overlayCharacters);

            //再配置
            RePositionOverlayCharacter();
        }

        private void RemovePlayer(Photon.Realtime.Player player)
        {
            foreach (var overlayCharacter in _overlayCharacters)
            {
                if (!Equals(overlayCharacter.Player, player)) continue;
                _overlayCharacters.Remove(overlayCharacter);
                Destroy(overlayCharacter.gameObject);
                break;
            }
        }

        private List<OverlayCharacter> SortOverlayCharacters(List<OverlayCharacter> overlayCharacters)
        {
            //ClientMasterを先頭に持ってくる
            var masterClient =
                _overlayCharacters.FirstOrDefault(overlayCharacter => overlayCharacter.Player.IsMasterClient);
            _overlayCharacters.Remove(masterClient);
            _overlayCharacters.Insert(0, masterClient);

            return overlayCharacters;
        }

        private void RePositionOverlayCharacter()
        {
            for (var i = 0; i < _overlayCharacters.Count; i++)
            {
                _overlayCharacters[i].transform.SetParent(_overlayCharacterParent[i]);
                _overlayCharacters[i].transform.localPosition = _overlayCharacterPrefab.transform.localPosition;
            }
        }

        public void AllRemoveOverlayCharacter()
        {
            foreach (var overlayCharacter in _overlayCharacters)
                Destroy(overlayCharacter.gameObject);

            _overlayCharacters.Clear();
        }
    }
}