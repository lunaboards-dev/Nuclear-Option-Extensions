using System.Collections.Generic;

namespace NOX.RWRs;

static class RWR
{
    public static readonly Systems.Mask Helicopter = new(false, RWRThreatType.FCS | RWRThreatType.AAA | RWRThreatType.SAM | RWRThreatType.Ship);
    public static readonly Systems.Mask Fighter = new (true, RWRThreatType.AirIntercept);
    public static readonly Systems.Band Band = new();
    public static readonly Systems.Full Full = new();


    public static Dictionary<string, IRWRSystem> RWRSystems = new() {
        // The Cricket and Compass have the most basic RWRs
        {"COIN", Band},
        {"trainer", Band},

        // Helicopters get a helicopter specific RWR that only identifies ground threats.
        {"UtilityHelo1", Helicopter},
        {"AttackHelo1", Helicopter},
        {"QuadVTOL1", Helicopter},

        // Fighters only identify other fighters and have elevation
        {"Fighter1", Fighter},
        {"SmallFighter1", Fighter},

        // The Brawler, Ifrit, Medusa, and Darkreach all identify everything
        {"CAS1", Full},
        {"Multirole1", Full},
        {"EW1", Full},
        {"Darkreach", Full},
        {"fastBomber1", Full}
    };

    public static bool AddRWR(string unit, IRWRSystem rwr)
    {
        if (RWRSystems.ContainsKey(unit))
        {
            Plugin.Logger.LogWarning($"Attempt to overwrite RWR for unit {unit} with {rwr}.");
            return false;
        }
        RWRSystems.Add(unit, rwr);
        return true;
    }
}