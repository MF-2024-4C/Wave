using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using DG.Tweening;

namespace Wave.Result
{
    public abstract class ResultTextBase : MonoBehaviour
    {
        protected Tweener _tweener;
        public abstract void Hide(float animTime);
        public abstract void Hide();
        public abstract void StopAnimation();
        public virtual void Setup(List<ResultPlayerData> playerDataList){}
    }
}