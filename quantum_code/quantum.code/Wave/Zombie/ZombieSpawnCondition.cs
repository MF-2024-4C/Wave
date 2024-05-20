using System;

namespace Quantum;

public abstract unsafe partial class ZombieSpawnCondition : AssetObject
{
    public int ConditionID;
    
    public abstract bool CheckCondition(Frame f); 
}

[Serializable]
public unsafe partial class ZombieSpawnAreaCondition : ZombieSpawnCondition
{
    public override bool CheckCondition(Frame f)
    {
        return true;
    }
}