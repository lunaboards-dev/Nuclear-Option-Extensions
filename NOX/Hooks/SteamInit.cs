using HarmonyLib;
using Steamworks;

namespace NOX.Hooks;

[HarmonyPatch(typeof(SteamManager), "MarkInit")]
static class SteamInit
{
    static readonly EFriendFlags friend_flags = EFriendFlags.k_EFriendFlagImmediate;
    static bool InitComplete = false;
    internal static void DoSteamInit()
    {
        if (InitComplete) return;
        // we ball
        int count = SteamFriends.GetFriendCount(friend_flags);
        Plugin.Logger.LogDebug($"Steam reports {count} friends.");
        for (int i = 0; i < count; i++)
        {
            CSteamID friend = SteamFriends.GetFriendByIndex(i, friend_flags);
            if (friend.IsValid())
            {
                //Logger.LogDebug($"FRIEND {i}: {friend.m_SteamID}");
                Plugin.Friends.Add(friend.m_SteamID);
            }
        }
        Plugin.Logger.LogDebug($"Registered {Plugin.Friends.Count} friends.");
        InitComplete = true;
    }
    static void Postfix(SteamManager __instance, bool isServer)
    {
        if (!isServer)
        {
            DoSteamInit();
        }
    }
}