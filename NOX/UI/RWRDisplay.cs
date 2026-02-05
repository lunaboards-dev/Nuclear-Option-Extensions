using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using BepInEx;
using NOX.RWR;
using NuclearOption.MissionEditorScripts;
using UnityEngine;
using UnityEngine.UI;
using static NOX.RWR.Systems;

namespace NOX.UI;

public class RWRDisplay : MonoBehaviour
{
    public RectTransform Tf;
    CanvasRenderer Cr;
    RawImage RWRBase;
    Dictionary<RWRContact, GameObject> Contacts = [];
    ConditionalWeakTable<Unit, RWRContact> TrackedContacts = [];
    ConditionalWeakTable<Unit, RWRContact> TrackedMissiles = [];
    public RectTransform PRt;
    public RectTransform TRPanel;
    public IRWRSystem System;
    void Awake()
    {
        // Set up transform
        Tf = gameObject.AddComponent<RectTransform>();
        Tf.pivot = new Vector2(1, 1);
        Tf.SetRectSize(new Vector2(300, 300));
        Tf.anchorMax = new Vector2(1,1);
        Tf.anchorMin = new Vector2(1,1);

        Cr = gameObject.AddComponent<CanvasRenderer>();
        
        RWRBase = gameObject.AddComponent<RawImage>();
        RWRBase.texture = Resources.RWRBaseTex;
        RWRBase.material = new Material(Shader.Find("UI/Default"));
        RWRBase.color = Plugin.RWRColor.Value;

        System = FullRWR.Instance;
    }

    public RWRContact NewContact()
    {
        GameObject go = new GameObject("NOXRWRContact");
        RWRContact contact = go.AddComponent<RWRContact>();
        contact.Parent = this;
        contact.Tf.position = Tf.position;
        contact.label.transform.position = Tf.position;
        Contacts.Add(contact, go);
        go.transform.SetParent(Tf);
        go.SetActive(true);
        return contact;
    }

    internal void DestroyContact(RWRContact c)
    {
        Contacts[c].SetActive(false);
        Contacts.Remove(c);
        TrackedContacts.Remove(TrackedContacts.Where(item => item.Value == c).First().Key);
    }

    public RWRContact Ping(Unit unit)
    {
        if (!TrackedContacts.TryGetValue(unit, out RWRContact contact))
        {
            RWRContact c = NewContact();
            c.Tf.SetParent(Tf, false);
            TrackedContacts.Add(unit, c);
            contact = c;
        }
        var info = System.ThreatID(unit);
        if (contact == null)
        {
            Plugin.Logger.LogError($"Somehow, contact is null! {unit.name}");
            return null;
        }
        contact.UpdateData(info);
        contact.Renew();
        return contact;
    }

    public void Lock(Unit unit, bool status)
    {
        var contact = Ping(unit);
        contact.Lock = status;
        contact.tracked = unit;
    }

    public void Reset()
    {
        foreach (var pair in Contacts)
        {
            pair.Key.Destroy();
        }
        Contacts.Clear();
        TrackedContacts.Clear();
    }

    void Update()
    {
        //Plugin.Logger.LogInfo($"TRPanel: {TRPanel.anchoredPosition.y}-{TRPanel.rect.height} = {TRPanel.anchoredPosition.y-TRPanel.rect.height}");
        Tf.anchoredPosition = new Vector3(-Plugin.RWROffsetX.Value, TRPanel.anchoredPosition.y-TRPanel.rect.height-Plugin.RWROffsetY.Value);
        RWRBase.color = Plugin.RWRColor.Value;
        //Tf.position = new Vector2(150,150);
    }
}