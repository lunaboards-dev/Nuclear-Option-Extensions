using System;
using NuclearOption.MissionEditorScripts;
using UnityEngine;
using UnityEngine.UI;

namespace NOX.UI;

class RWRIndicator : MonoBehaviour
{
    RawImage contact;
    GameObject holder;
    Text text;
    public string Label
    {
        set => SetText(value);
        get => text.text;
    }
    public RectTransform Transform;

    public Vector2 Position
    {
        set
        {
            Transform.anchorMax = value;
            Transform.anchorMin = value;
            Transform.position = value;
            Transform.anchoredPosition = new Vector2(
                22-(value.x*44),
                22-(value.y*44)
            );
        }
    }

    float time_to_die = 0;

    public int Locks = 0;

    void SetText(string t)
    {
        text.text = t;
        switch(t.Length)
        {
            case 1:
                text.fontSize = 35;
                break;
            case 2:
                text.fontSize = 28;
                break;
            case 3:
                text.fontSize = 20;
                break;
            default:
                text.fontSize = 10;
                break;
        }
    }

    void Awake()
    {
        contact = gameObject.AddComponent<RawImage>();
        contact.texture = Resources.RWRContact;
        Transform = contact.rectTransform;
        Transform.SetRectSize(new Vector2(43, 43));
        holder = new GameObject("NOXIndicatorLabel");
        text = holder.AddComponent<Text>();
        text.alignment = TextAnchor.MiddleCenter;
        var center = new Vector2(0.5f, 0.5f);
        text.rectTransform.SetParent(Transform, false);
        text.rectTransform.anchorMax = center;
        text.rectTransform.anchorMin = center;
        text.rectTransform.pivot = center;
        text.rectTransform.anchoredPosition = Vector2.zero;
        text.font = Resources.Font;
        text.text = "lol";
    }

    public void Show(bool should)
    {
        contact.enabled = should;
        text.enabled = should;
        gameObject.SetActive(should);
        holder.SetActive(should);
    }

    public void Ping()
    {
        time_to_die = Time.timeSinceLevelLoad+3;
    }

    void Update()
    {
        text.rectTransform.anchoredPosition = Vector2.zero;
        //Plugin.Logger.LogInfo($"TRPanel: {TRPanel.anchoredPosition.y}-{TRPanel.rect.height} = {TRPanel.anchoredPosition.y-TRPanel.rect.height}");
        //Tf.position = new Vector2(150,150);
        if (Locks > 0)
        {
            Color c = Plugin.RWRColor.Value;//((Time.timeSinceLevelLoad*4)%1) > 0.5 ? Color.red : Color.yellow;
            contact.color = c;
            text.color = c;
        } else {
            float alpha = Mathf.Max((time_to_die-Time.timeSinceLevelLoad)/3, 0);
            Color c = Plugin.RWRColor.Value;
            c *= Mathf.Min(alpha*(3f/4f)+(1f/4f), 1f);
            c.a = Mathf.Min(alpha*(2f/3f)+(1f/3f), 1f);
            contact.color = c;
            text.color = c;
        }
    }
}