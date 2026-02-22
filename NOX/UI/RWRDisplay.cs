using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using BepInEx;
using NOX.RWRs;
using NuclearOption.MissionEditorScripts;
using UnityEngine;
using UnityEngine.UI;

namespace NOX.UI;

public class RWRDisplay : MonoBehaviour
{
    public RectTransform Tf;
    CanvasRenderer Cr;
    RawImage RWRBase;
    Dictionary<RWRContact, GameObject> Contacts = [];
    ConditionalWeakTable<Unit, RWRContact> TrackedContacts = [];
    ConditionalWeakTable<Missile, RWRContact> TrackedMissiles = [];
    public RectTransform PRt;
    public RectTransform TRPanel;
    public IRWRSystem System;
    GameObject ping_indicator;
    RawImage ping;
    public int Locks = 0;
    float time_to_die;
    RWRIndicator lo_ind;
    RWRIndicator i_ind;
    RWRIndicator j_ind;
    RWRIndicator hi_ind;

    RWRIndicator CreateIndicator(string label, Vector2 pos)
    {
        var obj = new GameObject("NOXBandIndicator");
        var ind = obj.AddComponent<RWRIndicator>();
        ind.Transform.SetParent(Tf, false);
        ind.Label = label;
        ind.Position = pos;
        return ind;
    }

    RWRIndicator GetIndicator(char band)
    {
        if (band < 'I')
        {
            return lo_ind;
        } else if (band == 'I')
        {
            return i_ind;
        } else if (band == 'J')
        {
            return j_ind;
        } else
        {
            return hi_ind;
        }
    }

    void AddLock(char band, bool locked)
    {
        var ind = GetIndicator(band);
        ind.Ping();
        ind.Locks = locked ? 1 : -1;
    }

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

        ping_indicator = new GameObject("NOXPingIndicator");
        ping = ping_indicator.AddComponent<RawImage>();
        ping.texture = Resources.RWRContact;
        ping.rectTransform.SetRectSize(new Vector2(43,43));
        ping.transform.SetParent(Tf);
        ping.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        ping.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        ping.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        ping.rectTransform.anchoredPosition = Vector2.zero;

        System = RWR.Full;

        lo_ind = CreateIndicator("LO", new Vector2(0, 0));
        hi_ind = CreateIndicator("HI", new Vector2(0, 1));
        i_ind = CreateIndicator("I", new Vector2(1, 0));
        j_ind = CreateIndicator("J", new Vector2(1, 1));
    }

    public void UpdateRWRDisplay()
    {
        lo_ind.Show(System.ShowContactLights);
        hi_ind.Show(System.ShowContactLights);
        i_ind.Show(System.ShowContactLights);
        j_ind.Show(System.ShowContactLights);
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

    public RWRContact Ping(Unit unit, bool detected = false, bool target = false)
    {
        if (!TrackedContacts.TryGetValue(unit, out RWRContact contact))
        {
            RWRContact c = NewContact();
            c.Tf.SetParent(Tf, false);
            c.line.transform.SetParent(Tf, false);
            TrackedContacts.Add(unit, c);
            contact = c;
        }
        var info = System.ThreatID(unit);
        if (contact == null)
        {
            Plugin.Logger.LogError($"Somehow, contact is null! {unit.name}");
            return null;
        }
        GetIndicator(info.Threat.Band[0]).Ping();
        contact.UpdateData(info, detected, target);
        contact.Renew();
        time_to_die = Time.timeSinceLevelLoad+3;
        return contact;
    }

    public void Lock(Unit unit, bool status)
    {
        Locks += status ? 1 : -1;
        var contact = Ping(unit, status, status);
        var threat = Threats.IdentifyThreat(unit);
        AddLock(threat.Band[0], status);
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
        Locks = 0;
        lo_ind.Locks = 0;
        hi_ind.Locks = 0;
        i_ind.Locks = 0;
        j_ind.Locks = 0;
    }

    void Update()
    {
        //Plugin.Logger.LogInfo($"TRPanel: {TRPanel.anchoredPosition.y}-{TRPanel.rect.height} = {TRPanel.anchoredPosition.y-TRPanel.rect.height}");
        Tf.anchoredPosition = new Vector3(-Plugin.RWROffsetX.Value, TRPanel.anchoredPosition.y-TRPanel.rect.height-Plugin.RWROffsetY.Value);
        RWRBase.color = Plugin.RWRColor.Value;
        //Tf.position = new Vector2(150,150);
        if (Locks > 0)
        {
            Color c = ((Time.timeSinceLevelLoad*4)%1) > 0.5 ? Color.red : Color.yellow;
            ping.color = c;
        } else {
            float alpha = Mathf.Max((time_to_die-Time.timeSinceLevelLoad)/3, 0);
            Color c = Plugin.RWRColor.Value;
            c.a = alpha;
            ping.color = c;
        }
    }
}