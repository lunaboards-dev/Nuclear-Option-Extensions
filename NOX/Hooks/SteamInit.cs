using HarmonyLib;
using Steamworks;

namespace NOX.Hooks;

[HarmonyPatch(typeof(SteamManager), "MarkInit")]
static class SteamInit
{
    static bool InitComplete = false;
    internal static void DoSteamInit()
    {
        if (InitComplete) return;
        // we ball
        Steam.Init();
        Steam.Instance.RefreshFriends();
        InitComplete = true;
        if (BetaTesters.LocalIsTester)
            Plugin.Logger.LogWarning("!!!!!!!!!!!! EXPERIMENTAL FEATURES ARE ENABLED !!!!!!!!!!!!");
    }

    static void Postfix(SteamManager __instance, bool isServer)
    {
        if (!isServer)
        {
            DoSteamInit();
            __instance.gameObject.AddComponent<Steam.SteamCallbackRunner>();
        }
    }
}