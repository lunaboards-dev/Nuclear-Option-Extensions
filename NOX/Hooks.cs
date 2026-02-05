namespace NOX;
using Steamworks;
using HarmonyLib;
using NuclearOption.Networking;
using System.Reflection;
using System;
using System.Collections;
using NOX.UI;
using System.Linq;
using UnityEngine;
using NOX.RWR;
using UnityEngine.UI;

[HarmonyPatch(typeof(SteamManager), "MarkInit")]
static class SteamInitHook
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

[HarmonyPatch(typeof(Player), nameof(Player.SetAircraft))]
static class AircraftSetHook
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

[HarmonyPatch(typeof(RadarWarning), "Update")]
static public class StrawberryJam
{
    private static readonly FieldInfo JammingLookupField = AccessTools.Field(typeof(RadarWarning), "jammingIconLookup");
    static void Postfix(RadarWarning __instance)
    {
        if (__instance == null || JammingLookupField == null || !(JammingLookupField.GetValue(__instance) is IDictionary dictionary))
        {
            return;
        }
        //var jams = Traverse.Create(__instance).Field("jammingIconLookup").GetValue<Dictionary<Unit, object>>();
        Plugin.PlayerJammed = dictionary.Keys.Count > 0;
    }
}

[HarmonyPatch(typeof(HUDUnitMarker), "UpdateColor")]
static public class HUDMarker
{
    static void Prefix(HUDUnitMarker __instance)
    {
        if (DynamicMap.GetFactionMode(__instance.unit.NetworkHQ) == FactionMode.Friendly &&
            Plugin.FriendsAircraft.ContainsValue(__instance.unit.persistentID)) {
            Traverse.Create(__instance).Field("color").SetValue(Plugin.SquadColor.Value);
        }
    }
}

[HarmonyPatch(typeof(FlightHud), "ResetAircraft")]
class ResetUnitDistanceDictOnRespawnPatch {
    static void Postfix() {
        foreach (var ent in Plugin.labels)
        {
            if (ent.Key == null || ent.Key.unit == null || !Plugin.FriendsAircraft.ContainsValue(ent.Key.unit.persistentID) || !ent.Key.unit.isActiveAndEnabled || ent.Key.unit.GetPlayer() == null)
            {
                try {
                    ent.Value?.label?.text = "";
                    ent.Value?.label?.enabled = false;
                } catch(Exception)
                {
                    Plugin.Logger.LogError("failed to disable label, somehow");
                }
            }
        }
        Plugin.LocalUnit = SceneSingleton<CombatHUD>.i.aircraft;
        AddRWRHUDApp.Instance?.Reset();
        RWRHelper.RegisterRWREvents();
    }
}

[HarmonyPatch(typeof(HeadMountedDisplay), "Start")]
class AddRWRHUDApp
{   
    //private static readonly FieldInfo appLookupField = AccessTools.Field(typeof(HUDAppManager), "apps");
    static GameObject target;
    public static RWRDisplay Instance;

    static void Prefix(HeadMountedDisplay __instance)
    {
        /*HUDApp[] apps = appLookupField.GetValue(__instance) as HUDApp[];
        if (apps.Contains(RWRDisplay.Instance)) return;
        HUDApp[] new_apps = [.. apps.AddItem(RWRDisplay.Instance)];
        appLookupField.SetValue(__instance, new_apps);
        Plugin.Logger.LogInfo("Successfully registered RWR HUDApp!");*/
        Resources.AllFonts();
        target = new GameObject("NOX_RWRDisplay");
        HeadMountedDisplay disp = SceneSingleton<HeadMountedDisplay>.i;
        Instance = target.AddComponent<RWRDisplay>();
        var Rt = disp.gameObject.GetComponent<RectTransform>();
        Instance.Tf.SetParent(Rt, false);
        target.SetActive(true);
        Instance.PRt = Rt;
        for (int i = 0; i < Rt.childCount; ++i) {
            var c = Rt.GetChild(i);
            if (c.gameObject.name == "TopRightPanel")
            {
                Instance.TRPanel = c.gameObject.GetComponent<RectTransform>();
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(Rt);
        /*var contact = Instance.NewContact();
        contact.SetPos(new Vector3(40, 40));
        contact.SetText("K");
        contact.Update();*/
        RWRHelper.RegisterRWREvents();
        
    }
}

static class RWRHelper
{
    internal static void RadarWarning_OnRadarWarning(Aircraft.OnRadarWarning warning)
    {
        AddRWRHUDApp.Instance.Ping(warning.emitter);
    }

    internal static void onMissileWarning(MissileWarning.OnMissileWarning w)
    {
        if (w.missile.GetSeekerType() == "SARH") {
            AddRWRHUDApp.Instance.Lock(w.missile.owner, true);
        } else if (w.missile.GetSeekerType() == "ARH")
        {
            AddRWRHUDApp.Instance.Lock(w.missile, true);
        }
    }
    internal static void offMissileWarning(MissileWarning.OffMissileWarning w)
    {
        if (w.missile.GetSeekerType() == "SARH") {
            AddRWRHUDApp.Instance.Lock(w.missile.owner, false);
        } else if (w.missile.GetSeekerType() == "ARH")
        {
            AddRWRHUDApp.Instance.Lock(w.missile, false);
        }
    }
    static Aircraft oldAC;
    public static void RegisterRWREvents()
    {
        ClearRWREvents();
        Aircraft ac = SceneSingleton<CombatHUD>.i.aircraft;
        Plugin.Logger.LogInfo($"Local aircraft: unit = {ac.unitName}, unique = {ac.UniqueName}, name = {ac.name}, map unique = {ac.MapUniqueName}");
        if (AddRWRHUDApp.Instance != null) {
            AddRWRHUDApp.Instance.System = Systems.RWRSystems[ac.name];
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