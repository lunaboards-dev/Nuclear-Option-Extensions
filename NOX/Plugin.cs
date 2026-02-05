using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using NuclearOption.Networking;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace NOX;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    internal static List<ulong> Friends = [];
    internal static Dictionary<ulong, PersistentID> FriendsAircraft = [];
    private Harmony _harmony;
    public static bool PlayerJammed;
    public static Font font;
    public static Unit LocalUnit;
    public class Label
    {
        internal Text label;
        internal float spawntime;
    }

    public static ConditionalWeakTable<HUDUnitMarker, Label> labels = [];
    public static Label GetRef(HUDUnitMarker marker)
    {
        return labels.GetOrCreateValue(marker);
    }

    public static Text GetLabel(HUDUnitMarker marker)
    {
        return GetRef(marker).label;
    }

    public static void SetLabel(HUDUnitMarker marker, Text label)
    {
        GetRef(marker).label = label;
    }

    #region Config keys

    internal static ConfigEntry<Color> SquadColor;
    internal static ConfigEntry<int> FontSize;
    internal static ConfigEntry<float> NameOffset;
    internal static ConfigEntry<Color> RWRColor;
    internal static ConfigEntry<float> RWROffsetX;
    internal static ConfigEntry<float> RWROffsetY;
    internal static ConfigEntry<float> RWRScaling;

    #endregion Config keys


    internal static Plugin Instance;

    private void Awake()
    {
        Instance = this;
        // Plugin startup logic
        Logger = base.Logger;

        SquadColor = Config.Bind("Colors", "Squad Color", Color.green);
        RWRColor = Config.Bind("Colors", "RWR Color", Color.green);
        FontSize = Config.Bind("Labels", "Player Name Font Size", 14);
        NameOffset = Config.Bind("Labels", "Player Name Offset", 5f);
        RWROffsetX = Config.Bind("Position", "RWR Offset X", 5f);
        RWROffsetY = Config.Bind("Position", "RWR Offset Y", 0f);
        RWRScaling = Config.Bind("Position", "RWR Distance Scale (km)", 50f);
        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        _harmony.PatchAll();

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        try
        {
            SteamInitHook.DoSteamInit();
        } catch (Exception)
        {
            Logger.LogWarning("Steam API not initalized yet, hopefully the hook will work...");
        }
        Resources.Init();
    }

    // quick and dirty hooks

    
}
