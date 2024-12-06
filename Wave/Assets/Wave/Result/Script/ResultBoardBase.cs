using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Wave.Result
{
    public abstract class ResultBoardBase : MonoBehaviour, IResultBoard
    {
        [SerializeField] private CanvasGroup _layerGroup;
        protected ResultGameData _resultGameData;
        protected List<ResultPlayerData> _resultPlayerDatas;

        private bool _isShow = false;
        private bool _isNext = false;
        private Coroutine _coroutine;
        private Action _onFinish;

        protected virtual void Awake()
        {
            _layerGroup.alpha = 0;
        }

        public void Skip()
        {
            if (_isShow)
            {
                StopCoroutine(_coroutine);
                SkipFunc();
                AnimationFinish();
            }
        }

        protected virtual void SkipFunc(){}

    public void Next()
    { 
        Skip();
        _isNext = true;
    }

        /// <summary>
        /// リザルト画面を表示させるアニメーション開始
        /// </summary>
        /// <param name="resultGameData"></param>
        /// <param name="resultPlayerDataList"></param>
        /// <param name="onFinish"></param>
        public void Show(ResultGameData resultGameData, List<ResultPlayerData> resultPlayerDataList, Action onFinish)
        {
            _resultGameData = resultGameData;
            _resultPlayerDatas = resultPlayerDataList;
            _isShow = true;
            _isNext = false;
            _onFinish = onFinish;
            _coroutine = StartCoroutine("ResultUpdate");
        }

        protected void AnimationFinish()
        {
            _onFinish?.Invoke();
            _onFinish = null;
            _isShow = false;
            _resultGameData = null;
            _resultPlayerDatas = null;
            _coroutine = null;
            Debug.Log("Finish Result Board Anim");
        }
        
        public bool IsNext => _isNext;

        /// <summary>
        /// リザルトアニメーションが始まった時に呼ばれる
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerator ResultUpdate();

        public void FadeOutResultBoard(float fadeTime)
        {
            FadeResultBoard(0, fadeTime);
        }

        public void FadeInResultBoard(float fadeTime)
        {
            FadeResultBoard(1, fadeTime);
        }

        private void FadeResultBoard(float targetAlpha, float fadeTime)
        {
            _layerGroup.DOFade(targetAlpha, fadeTime);
        }
        
        public virtual void SetUp(List<ResultPlayerData> playerDataList){}
    }
}