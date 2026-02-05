using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace NOX.RWR;

public static class Systems
{
    public static string[] RandomContacts = [
        "C22",
        "T30",
        "A19",
        "F20",
        "F12",
        "H90",
        "H46",
        "V49",
        "E25",
        "K67",
        "B81",
        "9K4",
        "R9",

        // random bullshit from IRL
        "SA2",
        "F14",
        // idk i'll add more later
    ];

    static string RandomContact()
    {
        return RandomContacts[UnityEngine.Random.RandomRangeInt(0, RandomContacts.Length)];
    }

    static string RandomLetter()
    {
        char letter = (char)('A' + UnityEngine.Random.RandomRangeInt(0, 26));
        return letter.ToString();
    }

    public struct RWRThreat
    {
        public string ID;
        public float Direction;
        public float Distance;
        public float Elevation;
        public bool Lock;
    }

    static (float Direction, float Distance, float Elevation) ThreatDirection(Unit self, Unit threat)
    {
        var pos1 = self.GlobalPosition();
        var pos2 = threat.GlobalPosition();

        var dif = pos2.AsVector3()-pos1.AsVector3();
        var dif_norm = self.transform.InverseTransformDirection(dif.normalized);
        var local = self.transform.InverseTransformPoint(pos2.AsVector3());
        var norm = local.normalized;

        //var dif = pos2-pos1;

        /*var normal = local.normalized;
        var dist_length = local.magnitude;
        var elevation = Math.Acos(normal.z);
        var azimuth = Math.Atan2(normal.y, normal.x);*/

        float dist_len = dif.magnitude;
        
        float azimuth = Mathf.Atan2(dif_norm.z, dif_norm.x);

        float elevation = Mathf.Rad2Deg * Mathf.Atan2(dif_norm.y, dif_norm.magnitude);

        return ((float)azimuth, dist_len, (float)elevation);
    }
    public interface IRWRSystem
    {
        public RWRThreat ThreatID(Unit unit);
    }

    public class MaskedRWR(bool PassElevation, Threats.RWRThreatType ThreatMask) : IRWRSystem
    {
        public RWRThreat ThreatID(Unit unit)
        {
            var threat = Threats.IdentifyThreat(unit);
            var direction = ThreatDirection(Plugin.LocalUnit, unit);
            if ((threat.Class & ThreatMask) > 0)
            {
                return new()
                {
                    Elevation = PassElevation ? direction.Elevation : 0,
                    Direction = direction.Direction,
                    Distance = direction.Distance,
                    ID = Plugin.PlayerJammed ? RandomContact() : threat.Display
                };
            }
            return new()
            {
                Elevation = PassElevation ? direction.Elevation : 0,
                Direction = direction.Direction,
                Distance = direction.Distance,
                ID = Plugin.PlayerJammed ? RandomLetter() : Threats.ThreatLookup[threat.Class]
            };
        }
    }

    public static MaskedRWR Helicopter = new(false, Threats.RWRThreatType.FCS | Threats.RWRThreatType.AAA | Threats.RWRThreatType.SAM | Threats.RWRThreatType.Ship);
    public static MaskedRWR Fighter = new (true, Threats.RWRThreatType.AirIntercept);

    public class BandRWR : IRWRSystem
    {
        public static BandRWR Instance = new();

        public RWRThreat ThreatID(Unit unit)
        {
            var threat = Threats.IdentifyThreat(unit);
            var direction = ThreatDirection(Plugin.LocalUnit, unit);
            return new()
            {
                Elevation = 0,
                Direction = direction.Direction,
                Distance = direction.Distance,
                ID = Plugin.PlayerJammed ? RandomLetter() : threat.Band
            };
        }
    }

    public class FullRWR : IRWRSystem
    {
        public static FullRWR Instance = new();
        public RWRThreat ThreatID(Unit unit)
        {
            var threat = Threats.IdentifyThreat(unit);
            var direction = ThreatDirection(Plugin.LocalUnit, unit);
            return new()
            {
                Elevation = direction.Elevation,
                Direction = direction.Direction,
                Distance = direction.Distance,
                ID = Plugin.PlayerJammed ? RandomContact() : threat.Display
            };
        }
    }

    public static Dictionary<string, IRWRSystem> RWRSystems = new() {
        // The Cricket and Compass have the most basic RWRs
        {"COIN", BandRWR.Instance},
        {"trainer", BandRWR.Instance},

        // Helicopters get a helicopter specific RWR that only identifies ground threats.
        {"UtilityHelo1", Helicopter},
        {"AttackHelo1", Helicopter},
        {"QuadVTOL1", Helicopter},

        // Fighters only identify other fighters and have elevation
        {"Fighter1", Fighter},
        {"SmallFighter1", Fighter},

        // The Brawler, Ifrit, Medusa, and Darkreach all identify everything
        {"CAS1", FullRWR.Instance},
        {"Multirole1", FullRWR.Instance},
        {"EW1", FullRWR.Instance},
        {"Darkreach", FullRWR.Instance},
        {"fastBomber1", FullRWR.Instance}
    };
}