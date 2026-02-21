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
        if (Parent == null) return;
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
        if (!FriendsAircraft.ContainsValue(Parent.unit.persistentID) || Parent?.unit.GetPlayer() == null || !Parent.unit.isActiveAndEnabled || Parent.unit.unitState > Unit.UnitState.Damaged)
        {
            Nullify();
            return;
        }
        if (Label.transform == null || Parent?.image.transform == null) return;
        Label.transform.position = Parent.image.transform.position + new UnityEngine.Vector3(0f, NameOffset.Value, 0f);
        Label.color = SquadColor.Value;
        Label.enabled = Parent.image.enabled && !Parent.selected && !PlayerJammed;
    }

    void Hook()
    {
        Parent.unit.onDisableUnit += HUDMarker_OnDisable;
    }

    public static HUDMarker CreateLabel(HUDUnitMarker marker)
    {
        GameObject go = new GameObject("NOXSquadMarker");
        HUDMarker mark = go.AddComponent<HUDMarker>();
        mark.Parent = marker;
        mark.Name = marker.unit.unitName;
        mark.Label.color = new Color(0,0,0,0);
        mark.Hook();
        go.SetActive(true);
        return mark;
    }

    public static void ClearLabels()
    {
        var c = Markers.ToArray();
        foreach (var o in c)
        {
            if (o != null) {
                var M = o.GetComponent<HUDMarker>();
                if (o.TryGetComponent<HUDMarker>(out HUDMarker comp)) {
                    M?.Nullify();
                }
            }
        }
        Markers.Clear();
    }

    public void HUDMarker_OnDisable(Unit unit)
    {
        Nullify();
    }

    public static void RefreshLabels()
    {
        CombatHUD i = SceneSingleton<CombatHUD>.i;
    }
}