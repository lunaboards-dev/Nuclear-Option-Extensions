namespace NOX.RWRs.Systems;

public class Mask(bool PassElevation, RWRThreatType ThreatMask) : IRWRSystem
{
    public bool SupportsFilters => true;
    public RWRThreat ThreatID(Unit unit)
    {
        var threat = Threats.IdentifyThreat(unit);
        var direction = Utils.ThreatDirection(Plugin.LocalUnit, unit);
        if ((threat.Class & ThreatMask) > 0)
        {
            return new()
            {
                Elevation = PassElevation ? direction.Elevation : 0,
                Direction = direction.Direction,
                Distance = direction.Distance,
                ID = Plugin.PlayerJammed ? Utils.RandomContact() : threat.Display
            };
        }
        return new()
        {
            Elevation = PassElevation ? direction.Elevation : 0,
            Direction = direction.Direction,
            Distance = direction.Distance,
            ID = Plugin.PlayerJammed ? Utils.RandomLetter() : Threats.ThreatLookup[threat.Class]
        };
    }
}
