using System;
using System.Linq;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NOX.UI;

class RespawnTime : MonoBehaviour
{
    GameObject holder;
    Text text;
    RectTransform tf;
    Factory[] current_factories = [];

    void Awake()
    {
        holder = new GameObject("NOXRespawnTimer");
        text = holder.AddComponent<Text>();
        tf = holder.GetComponent<RectTransform>();
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.UIFont;
        text.fontSize = 15;
    }

    void InternalAttach(AircraftSelectionMenu menu)
    {
        Plugin.Logger.LogInfo("attached text to "+menu);
        var tmpro = Traverse.Create(menu).Field("aircraftName").GetValue<TMP_Text>();
        AttachToTMP(tmpro);
        menu.OnSelectedAircraftChange += SelAircraftChange;
    }

    void SelAircraftChange(AircraftDefinition def)
    {
        current_factories = [.. FindObjectsOfType<Factory>().Where(F => {
            if (F.ProductionUnit == null) {
                Plugin.Logger.LogWarning("No production unit for factory!"); return false;
            }
            if (F.attachedUnit == null) {
                Plugin.Logger.LogWarning("No attached unit for factory!"); return false;
            }
            return F.ProductionUnit.name == def.name && DynamicMap.GetFactionMode(F.attachedUnit.NetworkHQ) == FactionMode.Friendly;
        })];
        Plugin.Logger.LogInfo($"Found {current_factories.Length} factories for {def.name}");
    }

    float GetSoonestProduction()
    {
        float soonest = float.PositiveInfinity;
        foreach (var f in current_factories)
        {
            float next = f.GetNextProduction(true);
            soonest = Mathf.Min(soonest, next);
        }
        return soonest;
    }

    void UpdateTimer(string t)
    {
        text.text = "Next aircraft:\n"+t;
    }

    void Update()
    {
        float next = GetSoonestProduction();
        if (float.IsInfinity(next))
        {
            UpdateTimer("Never");
        } else if (next < 0)
        {
            UpdateTimer("Soon(tm)");
        } else
        {
            var timespan = TimeSpan.FromSeconds(Mathf.Max(next, 0));
            UpdateTimer(timespan.ToString(@"mm\:ss"));
        }
    }

    void AttachToTMP(TMP_Text text)
    {
        tf.SetParent(text.transform, false);
        tf.pivot = new Vector2(1,0.5f);
        tf.anchorMax = new Vector2(1, 0.5f);
        tf.anchorMin = new Vector2(1, 0.5f);
        tf.anchoredPosition = new Vector2(0,0);
        tf.position = text.transform.position;
    }

    internal static void Attach(AircraftSelectionMenu menu)
    {
        var rt = menu.gameObject.AddComponent<RespawnTime>();
        rt.InternalAttach(menu);
    }
}