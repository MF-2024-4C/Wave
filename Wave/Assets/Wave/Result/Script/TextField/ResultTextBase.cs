using UnityEditor.Search;
using UnityEngine;
using DG.Tweening;

namespace Wave.Result
{
    public abstract class ResultTextBase : MonoBehaviour
    {
        protected Tweener _tweener;
        public abstract void Show(ResultPlayerData playerData, ResultGameData gameData, float animTime);
        public abstract void Hide(float animTime);
        public abstract void Hide();
        public abstract void StopAnimation();
    }
}