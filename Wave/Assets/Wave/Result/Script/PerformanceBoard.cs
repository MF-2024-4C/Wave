using System.Collections;

namespace Wave.Result
{
    public class PerformanceBoard : ResultBoardBase
    {
        protected override void SkipFunc()
        {
        }

        public override void Next()
        {
            base.Next();
        }
        
        protected override IEnumerator ResultUpdate()
        {
            yield return null;
        }
    }
}