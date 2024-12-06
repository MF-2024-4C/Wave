namespace Wave.Result
{
    public abstract class CompleteTextBase : ResultTextBase
    {
        public abstract override void Hide(float animTime);

        public abstract override void Hide();

        public abstract override void StopAnimation();
        
        public abstract void Show(ResultPlayerData playerData, ResultGameData gameData, float animTime);
    } }