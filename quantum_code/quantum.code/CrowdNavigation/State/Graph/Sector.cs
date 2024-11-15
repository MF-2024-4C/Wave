namespace Quantum.FF;

public struct Sector
{
    public Vector3Byte Size;
    
    public Sector(Vector3Byte size)
    {
        Size = size;
    }
}

public struct SectorNode
{
    public Sector Sector;
    public int SectorInWindowNodeOffset;
    public int SectorInWindowNodeCount;
    
    public SectorNode(Sector sector, int sectorInWindowNodeOffset, int sectorInWindowNodeCount)
    {
        Sector = sector;
        SectorInWindowNodeOffset = sectorInWindowNodeOffset;
        SectorInWindowNodeCount = sectorInWindowNodeCount;
    }
}