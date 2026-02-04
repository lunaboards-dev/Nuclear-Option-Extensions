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
    public IRWRSystem System;
    void Awake()
    {
        // Set up transform
        Tf = gameObject.AddComponent<RectTransform>();
        Tf.position = new Vector3(180, 360+180);
        Tf.SetRectSize(new Vector2(300, 300));
        Tf.anchorMax = new Vector2(0,0);
        Tf.anchorMin = new Vector2(0,0);

        Cr = gameObject.AddComponent<CanvasRenderer>();
        
        RWRBase = gameObject.AddComponent<RawImage>();
        RWRBase.texture = Resources.RWRBaseTex;
        RWRBase.material = new Material(Shader.Find("UI/Default"));
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
}