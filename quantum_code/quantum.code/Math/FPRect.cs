using Photon.Deterministic;

namespace Quantum;

public struct FPRect
{
    
    public FPVector2 P1;
    public FPVector2 P2;
    public FPVector2 P3;
    public FPVector2 P4;
    
    public FPVector2 Center;
    public FPBounds2 Bounds;

    public FPRect(FPVector2 center, FPVector2 extents, FP rotation)
    {
        Center = center;

        var dir1 = FPVector2.Rotate(extents, rotation);

        extents.Y *= FP.Minus_1;
        var dir2 = FPVector2.Rotate(extents, rotation);

        P1 = center + dir1;
        P2 = center + dir2;
        P3 = center - dir1;
        P4 = center - dir2;

        Bounds = new FPBounds2(center, new FPVector2(FPMath.Max(P1.X, P2.X, P3.X, P4.X) - center.X, FPMath.Max(P1.Y, P2.Y, P3.Y, P4.Y) - center.Y));
    }
}