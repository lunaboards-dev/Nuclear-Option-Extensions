using System;
using HarmonyLib;

namespace NOX.Hooks;

[HarmonyPatch(typeof(HUDUnitMarker), "UpdateColor")]
static public class HUDMarkerColor
{
    static void Prefix(HUDUnitMarker __instance)
    {
        if (!Plugin.Squads) return;
        if (DynamicMap.GetFactionMode(__instance.unit.NetworkHQ) == FactionMode.Friendly &&
            Plugin.FriendsAircraft.ContainsValue(__instance.unit.persistentID)) {
            Traverse.Create(__instance).Field("color").SetValue(Plugin.SquadColor.Value);
        }
    }
}

