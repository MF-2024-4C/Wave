using System;
using Quantum.Events;

namespace Quantum;

[Serializable]
public class DefenseMissionTriggerConfig : BaseItemConfig
{
    public Int32 EventID;

    public override void Execute(Frame f, EntityRef item, EntityRef player)
    {
        
    }

    public override void Release(Frame f, EntityRef item, EntityRef player)
    {
    }
}