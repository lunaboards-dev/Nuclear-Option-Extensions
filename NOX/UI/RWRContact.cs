using System;
using System.IO;
using BepInEx;
using NOX.Hooks;
using NOX.RWRs;
using NuclearOption.MissionEditorScripts;
using UnityEngine;
using UnityEngine.UI;

namespace NOX.UI;

public class RWRContact : MonoBehaviour
{
    public RectTransform Tf;
    CanvasRenderer Cr;
    RawImage RWRBase;
    GameObject label_holder;
    GameObject track_line;
    //Image line;
    public RawImage line;
    public Text label;
    public RWRDisplay Parent;
    float time_to_die;
    public bool Lock;
    public Unit tracked;
    void Awake()
    {
        // Set up transform
        Tf = gameObject.AddComponent<RectTransform>();
        Tf.position = new Vector3(0, 0);
        Tf.SetRectSize(new Vector2(43, 43));
        Tf.anchorMax = new Vector2(0.5f,0.5f);
        Tf.anchorMin = new Vector2(0.5f,0.5f);

        Cr = gameObject.AddComponent<CanvasRenderer>();
        
        RWRBase = gameObject.AddComponent<RawImage>();
        RWRBase.texture = Resources.RWRContact;
        RWRBase.material = new Material(Shader.Find("UI/Default"));
        RWRBase.color = Color.green;

        label_holder = new GameObject("NOXContactLabel");
        label = label_holder.AddComponent<Text>();
        label_holder.transform.SetParent(Tf);

        //label = gameObject.AddComponent<Text>();
        label.font = Resources.Font;
        label.text = "RDR";
        label.fontSize = 12;
        label.alignment = TextAnchor.MiddleCenter;
        label.transform.SetParent(Tf);
        label.color = Color.green;
        label.enabled = true;

        label_holder.SetActive(true);

        track_line = new GameObject("NOXContactLine");
        line = track_line.AddComponent<RawImage>();
        line.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        line.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        line.rectTransform.pivot = new Vector2(0.5f, 0);
        line.texture = Resources.RWRLineDash;
        line.color = Color.green;
        line.enabled = true;

        track_line.SetActive(true);

        time_to_die = Time.timeSinceLevelLoad+3;
    }

    public void UpdateData(RWRThreat contact, bool detected, bool locked)
    {
        float x = Mathf.Cos(contact.Direction);
        float y = Mathf.Sin(contact.Direction);
        float dist = Mathf.Max(Mathf.Min(30f+((contact.Distance/(Plugin.RWRScaling.Value*1000))*90), 120), 30);

        var linelen = dist-41;
        line.rectTransform.SetRectSize(new Vector2(3, linelen));
        line.uvRect = new Rect(0, 0, 1, linelen/16);

        line.enabled = detected | locked;
        line.texture = locked ? Texture2D.whiteTexture : Resources.RWRLineDash;

        //track_line.transform.position = Parent.Tf.position + (new Vector3(x, y, 0)*(dist/2));
        line.rectTransform.anchoredPosition = new Vector2(x*21,y*21);
        line.rectTransform.eulerAngles = new Vector3(0, 0, contact.Direction*Mathf.Rad2Deg-90);

        SetPos(new Vector3(x, y, 0)*dist);
        SetText(contact.ID);
        if (contact.Elevation <= -10f)
        {
            RWRBase.texture = Resources.RWRContactLo;
        } else if (contact.Elevation >= 10f)
        {
            RWRBase.texture = Resources.RWRContactHi;
        } else
        {
            RWRBase.texture = Resources.RWRContact;
        }
        Renew();
    }

    public void SetPos(Vector3 pos)
    {
        Tf.anchoredPosition = pos;
    }

    public void SetText(string t)
    {
        label.text = t;
        switch(t.Length)
        {
            case 1:
                label.fontSize = 35;
                break;
            case 2:
                label.fontSize = 28;
                break;
            case 3:
                label.fontSize = 20;
                break;
            default:
                label.fontSize = 10;
                break;
        }
    }

    public void Update()
    {
        if (Lock)
        {
            Color c = ((Time.timeSinceLevelLoad*4)%1) > 0.5 ? Color.red : Color.yellow;
            label.color = c;
            RWRBase.color = c;
            line.color = c;
            if (tracked != null)
            {
                if (Plugin.LocalUnit?.transform == null || tracked?.transform == null) return;
                var threat = RWRHud.Instance.System.ThreatID(tracked);
                if (tracked is Missile)
                {
                    threat.ID = "MSL";
                }
                UpdateData(threat, true, true);
            }
        } else {
            float alpha = Mathf.Max((time_to_die-Time.timeSinceLevelLoad)/3, 0);
            Color c = Plugin.RWRColor.Value;
            c.a = alpha;
            label.color = c;
            RWRBase.color = c;
            line.color = c;
        }
        label.transform.position = Tf.position;
    }

    public void Renew()
    {
        time_to_die = Time.timeSinceLevelLoad+3;
        
    }

    public void Destroy()
    {
        if (Parent == null) return;
        Lock = false;
        time_to_die = 0;
        label.enabled = false;
        RWRBase.enabled = false;
    }
}