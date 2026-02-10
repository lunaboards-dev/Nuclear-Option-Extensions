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
using NOX.Hooks;
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

    #region Config keys

    internal static bool Squads => EnableSquadMarker.Value;
    internal static bool RWR => EnableRWRHud.Value;
    internal static bool FriendsMenu => EnableFriendsMenu.Value;

    internal static ConfigEntry<bool> EnableSquadMarker;
    internal static ConfigEntry<bool> EnableRWRHud;
    internal static ConfigEntry<bool> EnableFriendsMenu;
    internal static ConfigEntry<bool> LoadExternal;
    
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

        EnableSquadMarker = Config.Bind("Features", "Enable squad marker", true);
        EnableRWRHud = Config.Bind("Features", "Enable RWR HUD element", true);
        EnableFriendsMenu = Config.Bind("Features", "Enable Friends menu", false);
        LoadExternal = Config.Bind("Features", "Load external assets", false, "WARNING: This may break if installed from a mod manager");

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
            // idk sometime i had this works
            SteamInit.DoSteamInit();
        } catch (Exception)
        {
            Logger.LogWarning("Steam API not initalized yet, retrying on hook...");
        }
        Resources.Init();
        FactionHQ hq;
    }

    // quick and dirty hooks

    
}
