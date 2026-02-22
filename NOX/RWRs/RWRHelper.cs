using NOX.Hooks;

namespace NOX.RWRs;
static class RWRHelper
{
    internal static void RadarWarning_OnRadarWarning(Aircraft.OnRadarWarning warning)
    {
        RWRHud.Instance.Ping(warning.emitter, warning.detected, warning.isTarget);
    }

    internal static void onMissileWarning(MissileWarning.OnMissileWarning w)
    {
        if (w.missile.GetSeekerType() == "SARH") {
            RWRHud.Instance.Lock(w.missile.owner, true);
        } else if (w.missile.GetSeekerType() == "ARH")
        {
            RWRHud.Instance.Lock(w.missile, true);
        }
    }
    internal static void offMissileWarning(MissileWarning.OffMissileWarning w)
    {
        if (w.missile.GetSeekerType() == "SARH") {
            RWRHud.Instance.Lock(w.missile.owner, false);
        } else if (w.missile.GetSeekerType() == "ARH")
        {
            RWRHud.Instance.Lock(w.missile, false);
        }
    }
    static Aircraft oldAC;
    public static void RegisterRWREvents()
    {
        ClearRWREvents();
        Aircraft ac = SceneSingleton<CombatHUD>.i.aircraft;
        if (ac == null) {
            Plugin.Logger.LogWarning("Aircraft is null, so we'll try again later.");
            return;
        };
        Plugin.Logger.LogInfo($"Local aircraft: unit = {ac.unitName}, unique = {ac.UniqueName}, name = {ac.name}, map unique = {ac.MapUniqueName}");
        if (RWRHud.Instance != null) {
            if (!RWR.RWRSystems.TryGetValue(ac.name, out IRWRSystem system))
                system = RWR.Full;
            RWRHud.Instance.System = system;
            RWRHud.Instance.UpdateRWRDisplay();
            RWRHud.Instance.Reset();
            ac.onRadarWarning += RadarWarning_OnRadarWarning;
            ac.GetMissileWarningSystem().onMissileWarning += onMissileWarning;
            ac.GetMissileWarningSystem().offMissileWarning += offMissileWarning;
            oldAC = ac;
        }
    }
    public static void ClearRWREvents()
    {
        if (oldAC == null) return;
        oldAC.onRadarWarning -= RadarWarning_OnRadarWarning;
        oldAC.GetMissileWarningSystem()?.onMissileWarning -= onMissileWarning;
        oldAC.GetMissileWarningSystem()?.offMissileWarning -= offMissileWarning;
    }
}