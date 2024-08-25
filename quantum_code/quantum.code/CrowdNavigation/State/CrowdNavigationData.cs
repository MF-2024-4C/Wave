using System;
using System.Runtime.CompilerServices;
using Photon.Deterministic;
using Quantum;

namespace Quantum;

public partial class CrowdNavigationData
{
    public FP MaxSlope = FP._10 * 4;
    public FP MaxHeight = FP._10;
    public FP Radius = FP._0_50;
    public Baked BakedData;
}

[Serializable]
public struct Baked
{
    public int Width;
    public int Height;
    
}