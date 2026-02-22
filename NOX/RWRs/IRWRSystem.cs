namespace NOX.RWRs;

public interface IRWRSystem
{
    public bool SupportsFilters { get; }
    public string SystemName { get; }
    public bool ShowContactLights { get; }
    public RWRThreat ThreatID(Unit unit);
}