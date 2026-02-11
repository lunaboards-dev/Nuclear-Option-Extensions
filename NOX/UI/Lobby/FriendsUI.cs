using System.Collections.Generic;
using System.IO;
using NuclearOption.Jobs;
using NuclearOption.MissionEditorScripts;
using Steamworks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NOX.UI.Lobby;

class FriendsUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Image Rect;
    Outline Otl;
    EventTrigger Trg;

    public RectTransform Tf;

    GameObject test;
    int pos = 80;

    void Awake()
    {
        Tf = gameObject.AddComponent<RectTransform>();
        Rect = gameObject.AddComponent<Image>();
        Tf.anchorMax = new Vector2(1, 1);
        Tf.anchorMin = new Vector2(1, 1);
        Tf.pivot = new Vector2(0, 1);
        //Tf.sizeDelta = new Vector2(30, 1);

        Rect.material = new Material(Shader.Find("UI/Default"));
        Rect.color = Color.black;

        Otl = gameObject.AddComponent<Outline>();
        Otl.effectColor = Color.white;
        Otl.effectDistance = new Vector2(1, -1);

        //Tf.sizeDelta = new Vector2(300, 1080);
        Tf.anchoredPosition = new Vector2(0, 0);

        Trg = gameObject.AddComponent<EventTrigger>();
    }

    public void SetParent(Transform PTf)
    {
        Tf.SetParent(PTf);
        var PRt = PTf.AsRectTransform();
        //Tf.sizeDelta = new Vector2(PRt.rect.width * (300/1920), PRt.rect.height);
        Tf.SetRectSize(new Vector2(500, PRt.rect.height));
        //Tf.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, PRt.rect.height);
        
        //Plugin.Logger.LogInfo($"FriendsUI X: {Tf.position.x} Y: {Tf.position.y}, parent size: {PRt.rect.width}, {PRt.rect.height}");
        Tf.anchoredPosition = new Vector2(-90, 5);

        Steam.Instance.RefreshFriends(true);

        for (int i = 0; i < Steam.Instance.Friends.Count; ++i)
        {
            var f = Steam.Instance.Friends[i];
            AddUser(f);
        }
    }

    void AddUser(CSteamID id)
    {
        test = new GameObject("NOXFriendButton");
        Friend f = test.AddComponent<Friend>();
        f.Tf.SetParent(Tf, false);
        f.Tf.localPosition = new Vector3(0, -(pos+5), 0);
        f.SetID(id);
        pos+=85;
    }

    void OnMouseOver()
    {
        Tf.anchoredPosition = new Vector2(-500, 0);
    }

    void OnMouseExit()
    {
        Tf.anchoredPosition = new Vector2(-90, 0);
    }

    void Update()
    {
        var PRt = Tf.parent.AsRectTransform();
        //Tf.sizeDelta = new Vector2(PRt.rect.width * (300/1920), PRt.rect.height);
        //Plugin.Logger.LogInfo($"FriendsUI size: {Tf.rect.width}, {Tf.rect.width}");
        //Plugin.Logger.LogInfo($"FriendsUI X: {Tf.position.x} Y: {Tf.position.y}, parent size: {Tf.parent.AsRectTransform().rect.width}, {Tf.parent.AsRectTransform().rect.height}");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseOver();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseExit();
    }
}