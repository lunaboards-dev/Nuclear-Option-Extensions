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

[HarmonyPatch(typeof(VirtualMFD), nameof(VirtualMFD.SetupButtons))]
static public class RWRMFDButton
{
    public static Sprite sprite;
    static void Postfix(VirtualMFD __instance)
    {
        if (!Plugin.RWR) return;
        //ConfigPanel.CreatePanel(__instance);
        //FindFirstFree(__instance);
        GameObject obj = __instance.gameObject;
        GameObject screens = obj.GetChildByName("LeftScreens");
        var img = screens.GetChildComponentByName<Image>("FactionInfoPanel_Left");
        sprite = img.sprite;
    }
}