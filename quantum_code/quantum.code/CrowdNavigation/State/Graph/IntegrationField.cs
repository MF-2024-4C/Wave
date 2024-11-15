namespace Quantum.FF;

public struct IntegrationField
{
    public bool Enqueued;
    public float IntegratedCost;
    
    public IntegrationField(float integratedCost, bool enqueued)
    {
        Enqueued = enqueued;
        IntegratedCost = integratedCost;
    }
}