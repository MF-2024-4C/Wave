using Quantum;
using UnityEngine;

namespace Wave
{
    public static class MonoBehaviourExtension
    {
        /// <summary>
        /// EntityViewがローカルプレイヤーかどうかをチェックする。
        /// </summary>
        /// <param name="monoBehaviour"></param>
        /// <param name="entityView">PlayerLinkがアタッチされたものであること。</param>
        /// <returns>ローカルであるか</returns>
        public static bool IsLocal(this MonoBehaviour monoBehaviour, EntityView entityView)
        {
            var frame = QuantumRunner.Default.Game.Frames.Predicted;

            if (!frame.TryGet<PlayerLink>(entityView.EntityRef, out var playerLink))
            {
                return false;
            }

            return QuantumRunner.Default.Game.PlayerIsLocal(playerLink.Player);
        }

        /// <summary>
        /// 渡されたEntityViewの持つPlayerLinkが指定したプレイヤーと同じかどうかをチェックする。
        /// </summary>
        /// <param name="monoBehaviour"></param>
        /// <param name="entityView">PlayerLinkがアタッチされたものであること。</param>
        /// <param name="player">確認したいPlayerRef</param>
        /// <returns></returns>
        public static bool EqualsPlayer(this MonoBehaviour monoBehaviour, EntityView entityView, PlayerRef player)
        {
            var frame = QuantumRunner.Default.Game.Frames.Predicted;

            if (!frame.TryGet<PlayerLink>(entityView.EntityRef, out var playerLink))
            {
                return false;
            }

            return player == playerLink.Player;
        }
    }
}