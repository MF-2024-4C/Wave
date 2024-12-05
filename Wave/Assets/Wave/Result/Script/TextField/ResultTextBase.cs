using UnityEditor.Search;
using UnityEngine;

namespace Wave.Result
{
    public abstract class ResultTextBase : MonoBehaviour
    {
        public abstract void Show(ResultPlayerData playerData, ResultGameData gameData, float animTime);
        public abstract void Hide(float animTime);
        public abstract void Hide();
    }
}