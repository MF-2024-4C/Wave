namespace Quantum.FF;

/// <summary>
/// チャンク間の繋がりをグラフで表すための要素
/// </summary>
public struct Portal
{
    public Vector3Int Position;
    public int ConnectionLinkOffset;
    public int ConnectionLinkSize;

    public Portal(Vector3Int position, int connectionLinkOffset)
    {
        this.Position = position;
        ConnectionLinkOffset = connectionLinkOffset;
        ConnectionLinkSize = 0;
        
    }
}