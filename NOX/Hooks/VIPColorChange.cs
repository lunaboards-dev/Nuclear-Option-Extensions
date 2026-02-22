using HarmonyLib;

namespace NOX.Hooks;
/*
[HarmonyPatch(typeof(HUDUnitMarker), "UpdatePosition")]
static public class VIPColorChange
{
    static void Postfix(HUDUnitMarker __instance)
    {
        if (!Plugin.Squads) return;
        if (!Plugin.FriendsAircraft.ContainsValue(__instance.unit.persistentID) || DynamicMap.GetFactionMode(__instance.unit.NetworkHQ) != FactionMode.Friendly) return;
        Aircraft ac = __instance.unit as Aircraft;
        if (ac == null || ac.Player == null) return;
        
    }
}*/