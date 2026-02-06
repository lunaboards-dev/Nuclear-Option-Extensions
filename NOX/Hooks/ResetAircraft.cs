using System;
using HarmonyLib;
using NOX.RWRs;

namespace NOX.Hooks;

[HarmonyPatch(typeof(FlightHud), "ResetAircraft")]
public class ResetAircraft
{
    static void Postfix() {
        Plugin.LocalUnit = SceneSingleton<CombatHUD>.i.aircraft;
        if (Plugin.RWR) {
            RWRHud.Instance?.Reset();
            RWRHelper.RegisterRWREvents();
        }
        UI.HUDMarker.ClearLabels();
    }
}
