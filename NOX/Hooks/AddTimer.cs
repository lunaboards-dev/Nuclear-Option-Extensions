using System;
using HarmonyLib;
using NOX.UI;
using NuclearOption.Networking;

namespace NOX.Hooks;

[HarmonyPatch(typeof(AircraftSelectionMenu), "Initialize")]
static public class AddTimer
{
    static void Postfix(AircraftSelectionMenu __instance, Player localPlayer, Airbase airbase)
    {
        if (!__instance.gameObject.TryGetComponent(out RespawnTime t))
        {
            RespawnTime.Attach(__instance);
        }
    }
}