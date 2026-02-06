namespace NOX.UI;
using HarmonyLib;
using NuclearOption.Networking;
using System.Reflection;
using System;
using UnityEngine.UI;
using UnityEngine;
using static NOX.Plugin;
using System.Collections.Generic;

class HUDMarker : MonoBehaviour
{
    public static List<GameObject> Markers = [];
    Text Label;
    public HUDUnitMarker Parent;
    public String Name
    {
        get
        {
            return CachedName;
        }
        set
        {
            if (Parent == null) return;
            CachedName = value;
            Label.text = value;
            Label.enabled = true;
        }
    }

    string CachedName = "";

    void Awake()
    {
        Label = gameObject.AddComponent<Text>();
        gameObject.transform.SetParent(SceneSingleton<CombatHUD>.i.iconLayer, false);
        Label.font = NOX.Resources.Font;
        Label.fontSize = Plugin.FontSize.Value;
        Label.alignment = TextAnchor.UpperCenter;
        Markers.Add(gameObject);
    }

    void Nullify()
    {
        Parent = null;
        try {
            Label.text = "";
            Label.enabled = false;
        } catch {
            // idk pound sand
        }
        enabled = false;
        gameObject.SetActive(false);
        Markers.Remove(gameObject);
    }

    void Update()
    {
        if (Parent == null) return;
        if (!FriendsAircraft.ContainsValue(Parent.unit.persistentID) || Parent.unit.GetPlayer() == null || !Parent.unit.isActiveAndEnabled)
        {
            Nullify();
            return;
        }
        Label.transform.position = Parent.image.transform.position + new UnityEngine.Vector3(0f, NameOffset.Value, 0f);
        Label.color = SquadColor.Value;
        Label.enabled = Parent.image.enabled && !Parent.selected && !PlayerJammed;
    }

    public static HUDMarker CreateLabel(HUDUnitMarker marker)
    {
        GameObject go = new GameObject("NOXSquadMarker");
        HUDMarker mark = go.AddComponent<HUDMarker>();
        mark.Parent = marker;
        mark.Name = marker.unit.unitName;
        mark.enabled = true;
        go.SetActive(true);
        return mark;
    }

    public static void ClearLabels()
    {
        foreach (var o in Markers)
        {
            var M = o.GetComponent<HUDMarker>();
            M?.Nullify();
        }
        Markers.Clear();
    }

    public static void RefreshLabels()
    {
        CombatHUD i = SceneSingleton<CombatHUD>.i;
    }
}

class HUDMarkerHooks
{
    [HarmonyPatch(typeof(HUDUnitMarker), "UpdateVisibility")]
    static public class HUDLabelCreate
    {
        static void Postfix(HUDUnitMarker __instance)
        {
            /*foreach (var ent in Plugin.labels)
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
            }*/
            if (!Plugin.FriendsAircraft.ContainsValue(__instance.unit.persistentID)) return;
            Aircraft ac = __instance.unit as Aircraft;
            if (ac == null || ac.Player == null) return;
            HUDMarker.CreateLabel(__instance);
        }
    }

    /*[HarmonyPatch(typeof(HUDUnitMarker), "UpdatePosition")]
    public static class HudLabelUpdate
    {
        static void Postfix(HUDUnitMarker __instance)
        {
            Text lab = GetLabel(__instance);
            if (lab != null)
            {
                lab.transform.position = __instance.image.transform.position + new UnityEngine.Vector3(0f, NameOffset.Value, 0f);
                
            }
        }
    }*/

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