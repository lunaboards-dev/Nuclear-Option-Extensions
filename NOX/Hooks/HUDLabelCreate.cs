using System;
using HarmonyLib;
using NOX.UI;

namespace NOX.Hooks;

[HarmonyPatch(typeof(HUDUnitMarker), "UpdateVisibility")]
static public class HUDLabelCreate
{
    static void Postfix(HUDUnitMarker __instance)
    {
        if (!Plugin.FriendsAircraft.ContainsValue(__instance.unit.persistentID)) return;
        Aircraft ac = __instance.unit as Aircraft;
        if (ac == null || ac.Player == null) return;
        HUDMarker.CreateLabel(__instance);
    }
}