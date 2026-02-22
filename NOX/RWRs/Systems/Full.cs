namespace NOX.RWRs.Systems;

public class Full : IRWRSystem
{
    public bool SupportsFilters => true;
    public string SystemName => "AN/AWS-81";
    public bool ShowContactLights => false;
    public RWRThreat ThreatID(Unit unit)
    {
        var threat = Threats.IdentifyThreat(unit);
        var direction = Utils.ThreatDirection(Plugin.LocalUnit, unit);
        return new()
        {
            Elevation = direction.Elevation,
            Direction = direction.Direction,
            Distance = direction.Distance,
            ID = Plugin.PlayerJammed ? Utils.RandomContact() : threat.Display,
            Threat = threat
        };
    }
}