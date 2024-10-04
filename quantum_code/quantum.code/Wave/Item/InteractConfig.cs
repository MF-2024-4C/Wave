using Photon.Deterministic;
using Quantum.Inspector;

namespace Quantum
{
    public partial class InteractConfig
    {
        [Tooltip("ホールドでインタラクトするかどうか？")]
        public QBoolean IsHoldInteract;
        
        [Tooltip("ホールドでインタラクトする場合の時間")]
        public FP HoldTime;

        [Tooltip("インタラクト終了後に再度インタラクト出来るようになる時間")]
        public FP InteractCoolTime = FP.FromFloat_UNSAFE(0.2f);
    }
}