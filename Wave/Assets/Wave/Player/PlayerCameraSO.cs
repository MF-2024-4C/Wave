using System;
using UnityEngine;

namespace Wave.Player
{
    [CreateAssetMenu(fileName = "PlayerCameraSO", menuName = "Wave/Player/PlayerCameraSO")]
    public class PlayerCameraSO : ScriptableObject
    {
        public Action<float> OnChangeCameraHeight;
    }
}
