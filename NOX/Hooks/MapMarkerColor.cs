using HarmonyLib;
using NOX;

namespace NOX.Hooks;

[HarmonyPatch(typeof(UnitMapIcon), "UpdateIcon")]
static public class MapMarkerColor
{
    static void Postfix(UnitMapIcon __instance)
    {
        if (DynamicMap.GetFactionMode(__instance.unit.NetworkHQ) == FactionMode.Friendly &&
            Plugin.FriendsAircraft.ContainsValue(__instance.unit.persistentID))
        {
            __instance.iconImage.color = Plugin.SquadColor.Value;
        }
    }
}