namespace NOX.RWRs;

public interface IRWRSystem
{
    public bool SupportsFilters { get; }
    public RWRThreat ThreatID(Unit unit);
}