using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Wave.Result
{
    public class CompleteBoard : ResultBoardBase
    {
        [SerializeField] private List<CompleteTextBase> _resultTexts = new List<CompleteTextBase>();
        [SerializeField] private float _textFadeTime = 0.5f;
        private ResultPlayerData _localPlayerData;

        protected override void Awake()
        {
            base.Awake();
            
            foreach (ResultTextBase resultText in _resultTexts)
            {
                resultText.Hide();
            }
        }
        
        protected override void SkipFunc()
        {
            foreach (CompleteTextBase resultText in _resultTexts)
            {
                resultText.StopAnimation();
                resultText.Show(_localPlayerData, _resultGameData, 0);
            }
        }
        
        protected override IEnumerator ResultUpdate()
        {
            _localPlayerData = null;
            foreach (var playerData in _resultPlayerDatas)
            {
                if (playerData.IsLocalPlayer)
                {
                    _localPlayerData = playerData;
                    break;
                }
            }

            foreach (CompleteTextBase resultText in _resultTexts)
            {
                resultText.Show(_localPlayerData, _resultGameData, _textFadeTime);
                yield return new WaitForSeconds(_textFadeTime);
            }
            
            AnimationFinish();
            yield return null;
        }
    }
}