using System.Collections.Generic;
using System.IO;

namespace NOX;

class BetaTesters
{
    static List<ulong> testers = [];

    internal static void LoadBetaTesters()
    {
        Stream str = Resources.GetStream("NOX.assets.beta_testers.txt");
        if (str == null) return;
        using var reader = new StreamReader(str);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            line = line.Trim();
            if (line.Length > 0)
            {
                ulong id = ulong.Parse(line);
                testers.Add(id);
            }
        }
    }

    public static bool IsTester(ulong id)
    {
        return Plugin.EnableExperimentalFeatures.Value && testers.Contains(id);
    }

    public static bool LocalIsTester => Steam.ID > 0 && IsTester(Steam.ID);
}