using System.Collections;

namespace Wave.Result
{
    public class PerformanceBoard : ResultBoardBase
    {
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
            yield return null;
        }
    }
}