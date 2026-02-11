using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BepInEx.Logging;

namespace NOX.RWRs;

public static class Threats
{
    public static ReadOnlyDictionary<RWRThreatType, string> ThreatLookup = new(new Dictionary<RWRThreatType, string>()
    {
        {RWRThreatType.AirIntercept, "AI"},
        {RWRThreatType.Attacker, "ATK"},
        {RWRThreatType.AEW, "AEW"},
        {RWRThreatType.SAM, "SAM"},
        {RWRThreatType.AAA, "AAA"},
        {RWRThreatType.Ship, "NVL"},
        {RWRThreatType.FCS, "FCS"},
        {RWRThreatType.EarlyWarning, "EWR"},
        {RWRThreatType.Missile, "MSL"},
        {RWRThreatType.Unknown, "?"}
    });

    internal static RWRThreatID Ship = new()
    {
        Name = "",
        Display = "NVL",
        Band = "F",
        Class = RWRThreatType.Ship
    };

    internal static RWRThreatID DefaultThreat = new()
    {
        Name = "",
        Display = "?",
        Band = "H",
        Class = RWRThreatType.Unknown
    };

    internal static RWRThreatID Missile = new()
    {
        Name = "",
        Display = "MSL",
        Band = "I",
        Class = RWRThreatType.Missile
    };

    // We will now support adding new threats.
    internal static List<RWRThreatID> ThreatList = [
        new RWRThreatID { // FS-12
            Name = "Fighter1",
            Display = "F12",
            Band = "I",
            Class = RWRThreatType.AirIntercept
        },
        new RWRThreatID { // FS-20
            Name = "SmallFighter1",
            Display = "F20",
            Band = "I",
            Class = RWRThreatType.AirIntercept
        },
        new RWRThreatID { // EW-25
            Name = "EW1",
            Band = "E",
            Display = "E25",
            Class = RWRThreatType.AEW
        },
        new RWRThreatID { // SFB-81
            Name = "Darkreach",
            Display = "B81",
            Band = "J",
            Class = RWRThreatType.Attacker
        },
        new RWRThreatID { // new fast bomber
            Name = "fastBomber1",
            Display = "FB1",
            Band = "J",
            Class = RWRThreatType.Attacker
        },
        new RWRThreatID { // Boltstrike
            Name = "RadarSAM1",
            Display = "9K4",
            Band = "K",
            Class = RWRThreatType.SAM
        },
        new RWRThreatID {
            Name = "HLT-R",
            Display = "HLT",
            Band = "C",
            Class = RWRThreatType.EarlyWarning
        },
        new RWRThreatID {
            Name = "Truck2-R",
            Display = "R9", //"MSV",
            Band = "C",
            Class = RWRThreatType.SAM
        },
        new RWRThreatID {
            Name = "RadarContainer1",
            Display = "R9",
            Band = "C",
            Class = RWRThreatType.SAM
        },
        new RWRThreatID {
            Name = "radarStation1",
            Display = "EWR",
            Band = "C",
            Class = RWRThreatType.EarlyWarning
        },
        new RWRThreatID {
            Name = "Truck2-RSAM",
            Display = "R9",
            Band = "J",
            Class = RWRThreatType.SAM
        },
        new RWRThreatID {
            Name = "Multirole1",
            Display = "K67",
            Band = "I",
            Class = RWRThreatType.AirIntercept
        },
        new RWRThreatID {
            Name = "SAMTurret1",
            Display = "9K4",
            Band = "K",
            Class = RWRThreatType.SAM
        },
        new RWRThreatID {
            Name = "SAMTrailer1",
            Display = "R9",
            Band = "J",
            Class = RWRThreatType.SAM
        }
    ];

    /*public static List<string> Ships = [
        "FleetCarrier1", "AssaultCarrier1", "Destroyer1",
        "Corvette1", "LandingCraft1"
    ];*/

    public static RWRThreatID IdentifyThreat(Unit unit)
    {
        if (unit is Missile)
            return Missile;
        if (unit is Ship) //if (Ships.Contains(unit.name))
            return Ship;
        foreach (var t in ThreatList)
        {
            if (t.Name == unit.name)
            {
                return t;
            }
        }
        Plugin.Logger.LogWarning("No warning for "+unit.name);
        return DefaultThreat;
    }

    public static bool RegisterThreat(RWRThreatID threat)
    {
        if (ThreatList.Where(t => t.Name == threat.Name).Count() > 0)
        {
            Plugin.Logger.LogWarning($"Attempt to overwrite threat info for unit {threat.Name}. Ignoring.");
            return false;
        }
        ;
        ThreatList.Add(threat);
        return true;
    }
}