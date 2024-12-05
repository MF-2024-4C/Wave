using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Wave.Result
{
    public class CompleteBoard : ResultBoardBase
    {
        [SerializeField] private List<ResultTextBase> _resultTexts = new List<ResultTextBase>();
        [SerializeField] private float _textFadeTime = 0.5f;

        public override void Skip()
        {
            base.Skip();
        }

        public override void Next()
        {
            base.Next();
        }
        
        protected override IEnumerator ResultUpdate()
        {
            foreach (ResultTextBase resultText in _resultTexts)
            {
                resultText.Hide();
            }
            
            ResultPlayerData localPlayerData = null;
            foreach (var playerData in _resultPlayerDatas)
            {
                if (playerData.IsLocalPlayer)
                {
                    localPlayerData = playerData;
                    break;
                }
            }

            foreach (ResultTextBase resultText in _resultTexts)
            {
                resultText.Show(localPlayerData, _resultGameData, _textFadeTime);
                yield return new WaitForSeconds(_textFadeTime);
            }
            
            Finish();
            yield return null;
        }
    }
}