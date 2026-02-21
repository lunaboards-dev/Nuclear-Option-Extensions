using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using NOX.UI;
using NOX.UI.RWRConfig;
using NuclearOption.Networking;
using UnityEngine;
using UnityEngine.UI;

namespace NOX.Hooks;

[HarmonyPatch(typeof(VirtualMFD), "Start")]
static public class RWRMFDButton
{
    public static Sprite sprite;
    static void Postfix(VirtualMFD __instance)
    {
        if (!Plugin.RWR || !BetaTesters.LocalIsTester) return;
        //ConfigPanel.CreatePanel(__instance);
        //FindFirstFree(__instance);
        /*GameObject obj = __instance.gameObject;
        GameObject screens = obj.GetChildByName("LeftScreens");
        var img = screens.GetChildComponentByName<Image>("FactionInfoPanel_Left");
        sprite = img.sprite;*/
        Plugin.Logger.LogInfo("Registering MFD panels");
        CustomMFDPanel.Register<ConfigPanel>(__instance);
        __instance.SetupButtons();
    }
}

[HarmonyPatch(typeof(VirtualMFD), "PressLeftButton")]
static public class RWRMFDButtonLP
{
    public static Sprite sprite;
    static void Postfix(VirtualMFD __instance, Button button)
    {
        Plugin.Logger.LogInfo($"Button pressed {button}");
    }
}

[HarmonyPatch(typeof(VirtualMFD), "PressRightButton")]
static public class RWRMFDButtonRP
{
    public static Sprite sprite;
    static void Postfix(VirtualMFD __instance, Button button)
    {
        Plugin.Logger.LogInfo($"Button pressed {button}");
    }
}