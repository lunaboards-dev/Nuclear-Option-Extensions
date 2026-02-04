namespace NOX.UI;
using HarmonyLib;
using NuclearOption.Networking;
using System.Reflection;
using System;
using UnityEngine.UI;
using UnityEngine;
using static NOX.Plugin;

class HUDMarker
{
    [HarmonyPatch(typeof(HUDUnitMarker), "UpdateVisibility")]
    static public class HUDLabelCreate
    {
        static void Postfix(HUDUnitMarker __instance)
        {
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
            if (!Plugin.FriendsAircraft.ContainsValue(__instance.unit.persistentID)) return;
            Aircraft ac = __instance.unit as Aircraft;
            if (ac == null || ac.Player == null) return;
            Label lref = Plugin.GetRef(__instance);
            Text text = lref.label;
            if (text == null)
            {
                GameObject go = new GameObject("HUD_PlayerName");
                go.transform.SetParent(SceneSingleton<CombatHUD>.i.iconLayer, false);
                text = go.AddComponent<Text>();
                text.font = NOX.Resources.Font;
                text.fontSize = Plugin.FontSize.Value;
                text.alignment = TextAnchor.UpperCenter;
                Plugin.SetLabel(__instance, text);
                lref.spawntime = Time.timeSinceLevelLoad;
            }
            text.text = ac.Player.PlayerName;
            if (Time.timeSinceLevelLoad - lref.spawntime < 0.01f)
            {
                text.enabled = false;
            }
        }
    }

    [HarmonyPatch(typeof(HUDUnitMarker), "UpdatePosition")]
    public static class HudLabelUpdate
    {
        static void Postfix(HUDUnitMarker __instance)
        {
            Text lab = GetLabel(__instance);
            if (lab != null)
            {
                lab.transform.position = __instance.image.transform.position + new UnityEngine.Vector3(0f, NameOffset.Value, 0f);
                lab.color = SquadColor.Value;
                lab.enabled = __instance.image.enabled && !__instance.selected && !PlayerJammed;
            }
        }
    }

    [HarmonyPatch(typeof(UnitMapIcon), "UpdateIcon")]
    static public class MapMarker
    {
        static void Postfix(UnitMapIcon __instance)
        {
            if (DynamicMap.GetFactionMode(__instance.unit.NetworkHQ) == FactionMode.Friendly &&
                FriendsAircraft.ContainsValue(__instance.unit.persistentID))
            {
                __instance.iconImage.color = SquadColor.Value;
            }
        }
    }
}