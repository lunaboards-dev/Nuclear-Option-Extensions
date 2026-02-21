namespace NOX.RWRs.Systems;

public class Full : IRWRSystem
{
    public bool SupportsFilters => true;
    public RWRThreat ThreatID(Unit unit)
    {
        var threat = Threats.IdentifyThreat(unit);
        var direction = Utils.ThreatDirection(Plugin.LocalUnit, unit);
        return new()
        {
            Elevation = direction.Elevation,
            Direction = direction.Direction,
            Distance = direction.Distance,
            ID = Plugin.PlayerJammed ? Utils.RandomContact() : threat.Display
        };
    }
}