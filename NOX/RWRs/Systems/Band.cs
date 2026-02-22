namespace NOX.RWRs.Systems;

public class Band : IRWRSystem
{
    //public static BandRWR Instance = new();

    public bool SupportsFilters => false;
    public string SystemName => "SPO-51";
    public bool ShowContactLights => false;

    public RWRThreat ThreatID(Unit unit)
    {
        var threat = Threats.IdentifyThreat(unit);
        var direction = Utils.ThreatDirection(Plugin.LocalUnit, unit);
        return new()
        {
            Elevation = 0,
            Direction = direction.Direction,
            Distance = direction.Distance,
            ID = Plugin.PlayerJammed ? Utils.RandomLetter() : threat.Band,
            Threat = threat
        };
    }
}