using System;
using System.Collections;
using System.Reflection;
using HarmonyLib;

namespace NOX.Hooks;

[HarmonyPatch(typeof(RadarWarning), "Update")]
static public class StrawberryJam
{
    private static readonly FieldInfo JammingLookupField = AccessTools.Field(typeof(RadarWarning), "jammingIconLookup");
    static void Postfix(RadarWarning __instance)
    {
        if (__instance == null || JammingLookupField == null || JammingLookupField.GetValue(__instance) is not IDictionary dictionary)
        {
            return;
        }
        //var jams = Traverse.Create(__instance).Field("jammingIconLookup").GetValue<Dictionary<Unit, object>>();
        Plugin.PlayerJammed = dictionary.Keys.Count > 0;
    }
}