namespace Quantum.FF;

public struct ConnectionLink
{
    public float Distance;
    public int LinkId;
    
    public ConnectionLink(float distance, int linkId)
    {
        Distance = distance;
        LinkId = linkId;
    }
}