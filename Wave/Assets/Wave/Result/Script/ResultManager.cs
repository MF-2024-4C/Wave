using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wave.Result
{
    public class ResultManager : MonoBehaviour
    {
        [SerializeField] private List<ResultBoardBase> _resultBoardList;
        [SerializeField] private float _resultChangeFadeTime = 0.5f;
        private bool _isShowResult = false;
        private ResultGameData _resultGameData;
        private ResultBoardBase _currentResultBoard;
        
        private List<ResultPlayerData> _resultPlayerDataList;
        private Coroutine _resultCoroutine;

        private void Start()
        {
            //何かしらイベントが発行されてリザルトを表示させる
            //今回はStartでリザルトを表示させる
            /*----Test Data----*/
            List<ResultPlayerData> resultPlayerDataList = new List<ResultPlayerData>();
            resultPlayerDataList.Add(new ResultPlayerData("Player1", 10, 1000.0f, true));
            resultPlayerDataList.Add(new ResultPlayerData("Player2", 5, 500.0f, false));
            resultPlayerDataList.Add(new ResultPlayerData("Player3", 7, 800.0f, false));
            resultPlayerDataList.Add(new ResultPlayerData("Player4", 4, 300.0f, false));
            ShowResult(new ResultGameData("Stage1", 314.0f), resultPlayerDataList);
            /*-----------------*/
        }

        public void ShowResult(ResultGameData resultGameData, List<ResultPlayerData> resultPlayerDataList)
        {
            _resultGameData = resultGameData;
            _resultPlayerDataList = resultPlayerDataList;
            _isShowResult = true;
            _resultCoroutine = StartCoroutine(ShowResultCoroutine());
        }
        
        public IEnumerator ShowResultCoroutine()
        {
            foreach (ResultBoardBase resultBoard in _resultBoardList)
            {
                resultBoard.gameObject.SetActive(false);
            }
            
            foreach (ResultBoardBase resultBoard in _resultBoardList)
            {
                _currentResultBoard = resultBoard;
                resultBoard.gameObject.SetActive(true);
                resultBoard.FadeInResultBoard(_resultChangeFadeTime);
                yield return new WaitForSeconds(_resultChangeFadeTime);
                
                resultBoard.Show(_resultGameData, _resultPlayerDataList);
                yield return new WaitUntil(() => resultBoard.IsNext);
                
                resultBoard.FadeOutResultBoard(_resultChangeFadeTime);
                yield return new WaitForSeconds(_resultChangeFadeTime);
                resultBoard.gameObject.SetActive(false);
            }

            _isShowResult = false;
            _resultGameData = null;
            Debug.Log("Finish Result");
        }

        public void SkipResult()
        {
            if(_isShowResult && _currentResultBoard != null)
            {
                _currentResultBoard.Skip();
            }
        }
        
        public void NextResult()
        {
            if(_isShowResult && _currentResultBoard != null)
            {
                _currentResultBoard.Next();
            }
        }
    }

    /// <summary>
    /// リザルトの渡されるプレイヤーデータ
    /// </summary>
    public class ResultPlayerData
    {
        public string PlayerName{ get;}
        public int KillCount    { get;}
        public float DamageDealt { get;}
        public bool IsLocalPlayer { get;}
        
        public ResultPlayerData(string playerName, int killCount,float damageDealt, bool isLocalPlayer)
        {
            PlayerName = playerName;
            KillCount = killCount;
            DamageDealt = damageDealt;
            IsLocalPlayer = isLocalPlayer;
        }
        
        public ResultPlayerData()
        {
            PlayerName = "None";
            KillCount = 0;
            DamageDealt = 0.0f;
            IsLocalPlayer = false;
        }
    }

    /// <summary>
    /// リザルトに渡されるゲームデータ
    /// </summary>
    public class ResultGameData
    {
        public string StageName { get; }
        public float ClearTime { get; }
        public ResultGameData (string stageName, float clearTime)
        {
            StageName = stageName;
            ClearTime = clearTime;
        }
        
        public ResultGameData()
        {
            StageName = "None";
            ClearTime = 9999.0f;
        }
    }
}