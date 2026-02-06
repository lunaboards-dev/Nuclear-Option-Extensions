using UnityEngine;

namespace NOX.RWRs;

static class Utils
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

    public static string RandomContact()
    {
        return RandomContacts[UnityEngine.Random.RandomRangeInt(0, RandomContacts.Length)];
    }

    public static string RandomLetter()
    {
        char letter = (char)('A' + UnityEngine.Random.RandomRangeInt(0, 26));
        return letter.ToString();
    }

    public static (float Direction, float Distance, float Elevation) ThreatDirection(Unit self, Unit threat)
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
}