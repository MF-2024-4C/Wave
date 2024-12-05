using System;
using System.Collections.Generic;

namespace Wave.Result
{
    public interface IResultBoard
    {
        public abstract void Skip();
        public abstract void Next();
        public abstract void Show(ResultGameData resultGameData, List<ResultPlayerData> resultPlayerDataList);
    }
}