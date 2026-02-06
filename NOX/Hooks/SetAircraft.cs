using System;
using HarmonyLib;
using NuclearOption.Networking;

namespace NOX.Hooks;

[HarmonyPatch(typeof(Player), nameof(Player.SetAircraft))]
static class SetAircraft
{
    static void Prefix(Player __instance, Aircraft aircraft)
    {
        ulong steamid = __instance.SteamID;
        if (Plugin.Friends.Contains(steamid))
        {
            Plugin.FriendsAircraft[steamid] = aircraft.persistentID;
        }
    }
}
