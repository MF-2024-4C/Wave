namespace Quantum.FF;

public struct ConnectionPoint
{
    public Portal Portal1;
    public Portal Portal2;
    
    public ConnectionPoint(Portal portal1, Portal portal2)
    {
        Portal1 = portal1;
        Portal2 = portal2;
    }
}