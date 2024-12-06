using System.Collections.Generic;

namespace Wave.Result
{
    public abstract class PerformanceTextBase : ResultTextBase
    {
        public abstract override void Hide(float animTime);

        public abstract override void Hide();

        public abstract override void StopAnimation();
        
        public abstract void Show(List<ResultPlayerData> playerDataList, ResultGameData gameData, float animTime);
    }
}