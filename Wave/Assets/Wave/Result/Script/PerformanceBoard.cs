using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wave.Result
{
    public class PerformanceBoard : ResultBoardBase
    {
        [SerializeField] private List<PerformanceTextBase> _resultTexts = new List<PerformanceTextBase>();
        [SerializeField] private float _textAnimTime = 0.5f;
        
        protected override void SkipFunc()
        {
            foreach (PerformanceTextBase resultText in _resultTexts)
            {
                resultText.StopAnimation();
                resultText.Show(_resultPlayerDatas, _resultGameData, 0);
            }
        }
        
        public override void SetUp(List<ResultPlayerData> playerDataList)
        {
            foreach (PerformanceTextBase resultText in _resultTexts)
            {
                resultText.Setup(playerDataList);
            }
        }
        
        protected override IEnumerator ResultUpdate()
        {
            foreach (PerformanceTextBase resultText in _resultTexts)
            {
                resultText.Show(_resultPlayerDatas, _resultGameData, _textAnimTime);
                yield return new WaitForSeconds(_textAnimTime);
            }
            
            AnimationFinish();
            yield return null;
        }
    }
}