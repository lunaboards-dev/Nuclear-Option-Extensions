using NOX.Scripting;

namespace NOX.RWRs;

public struct RWRContact
{
    public Unit Threat;
    public string Name;
    public string RadarBand;
    public string MissileSeeker;
    public string[] Groups;
    public double Distance;
    public double Angle;
    public double Elevation;

    public void Push(ScriptEnv L)
    {
        L.NewTable();
        L.PushString("unit");
        L.PushLightUserData((nint)Threat.GetHashCode());
        L.SetTable(-3);
        L.PushString("name");
        L.PushString(Name);
        L.SetTable(-3);
        L.PushString("band");
        L.PushString(RadarBand);
        L.SetTable(-3);
        if (MissileSeeker != null)
        {
            L.PushString("seeker");
            L.PushString(MissileSeeker);
            L.SetTable(-3);
        }
        L.PushString("distance");
        L.PushNumber(Distance);
        L.SetTable(-3);
        L.PushString("angle");
        L.PushNumber(Angle);
        L.SetTable(-3);
        L.PushString("elevation");
        L.PushNumber(Elevation);
        L.SetTable(-3);
        L.PushString("groups");
        L.NewTable();
        for (int i=0;i<Groups.Length;++i)
        {
            L.PushInteger(i+1);
            L.PushString(Groups[i]);
            L.SetTable(-3);
        }
        L.SetTable(-3);
    }
}