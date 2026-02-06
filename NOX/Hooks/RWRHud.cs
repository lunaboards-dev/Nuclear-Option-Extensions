using System;
using HarmonyLib;
using NOX.RWRs;
using NOX.UI;
using UnityEngine;
using UnityEngine.UI;

namespace NOX.Hooks;

[HarmonyPatch(typeof(HeadMountedDisplay), "Start")]
public class RWRHud
{
    static GameObject target;
    public static RWRDisplay Instance;

    static void Prefix(HeadMountedDisplay __instance)
    {
        if (!Plugin.RWR) return;
        UI.HUDMarker.Markers.Clear();
        Instance = null;
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
        //LayoutRebuilder.ForceRebuildLayoutImmediate(Rt);
        RWRHelper.RegisterRWREvents();
    }
}
