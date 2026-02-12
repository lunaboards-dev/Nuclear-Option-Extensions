using System.Collections.Generic;
using System.IO;
using NuclearOption.Jobs;
using NuclearOption.MissionEditorScripts;
using Steamworks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace NOX.UI.Lobby;

class FriendsUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Image Rect;
    Outline Otl;
    EventTrigger Trg;

    public RectTransform Tf;
    GameObject test;
    int pos = 80;

    GameObject scrollview;
    GameObject viewportholder;
    GameObject verticallayout;
    ScrollRect scroll;
    VerticalLayoutGroup Vlg;
    Mask viewport;

    ContentSizeFitter fitter;

    RectTransform STf;
    RectTransform VTf;
    RectTransform VPf;

    Color defcol = new(1,1,1,1);

    void AddDebugOutline(GameObject o, Color c)
    {
        var l = o.AddComponent<Outline>();
        l.effectColor = c;
        l.effectDistance = new Vector2(2, -2);
    }
    void Awake()
    {
        scrollview = new GameObject("NOXScrollView");
        scroll = scrollview.AddComponent<ScrollRect>();
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 7;
        scroll.verticalScrollbarSpacing = 0;
        scroll.horizontalScrollbarSpacing = 0;

        verticallayout = new GameObject("NOXVerticalLayout");
        Vlg = verticallayout.AddComponent<VerticalLayoutGroup>();
        Vlg.spacing = 5;
        Vlg.padding = new RectOffset(0,0,0,0);
        Vlg.childForceExpandHeight = true;
        Vlg.childForceExpandWidth = true;
        //Vlg.childControlHeight = true;
        //Vlg.childControlWidth = true;
        Vlg.childAlignment = TextAnchor.UpperLeft;
        fitter = verticallayout.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.MinSize;

        viewportholder = new GameObject("NOXViewport");
        viewport = viewportholder.AddComponent<Mask>();
        viewportholder.AddComponent<Image>().color = Color.black;

        Tf = gameObject.AddComponent<RectTransform>();
        VPf = viewportholder.GetComponent<RectTransform>();
        STf = scrollview.GetComponent<RectTransform>();
        VTf = verticallayout.GetComponent<RectTransform>();

        STf.SetParent(Tf, false);
        STf.anchorMax = new Vector2(0,1);
        STf.anchorMin = new Vector2(0,1);
        STf.pivot = new Vector2(0f, 1f);
        STf.anchoredPosition = new Vector2(0,0);
        VPf.SetParent(STf, false);
        VPf.anchorMax = new Vector2(1,1);
        VPf.anchorMin = new Vector2(0,0);
        VPf.pivot = new Vector2(0, 1);
        VPf.anchoredPosition = new Vector2(0,0);
        VTf.SetParent(STf, false);
        VTf.anchorMax = new Vector2(0,1);
        VTf.anchorMin = new Vector2(0,1);
        VTf.pivot = new Vector2(0, 1);
        VTf.anchoredPosition = new Vector2(0,0);

        //AddDebugOutline(scrollview, Color.red);
        //AddDebugOutline(viewportholder, Color.green);
        //AddDebugOutline(verticallayout, Color.blue);

        scroll.viewport = VPf;
        scroll.content = VTf;
        scroll.vertical = true;

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
        STf.SetRectSize(new Vector2(500, PRt.rect.height));
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
        Vlg.enabled = false;
        f.Tf.SetParent(VTf, false);
        
        f.SetID(id);
        RefreshLayout();
        Vlg.enabled = true;
    }

    void RefreshLayout()
    {
        Vlg.CalculateLayoutInputVertical();
        LayoutRebuilder.ForceRebuildLayoutImmediate(VPf);
        LayoutRebuilder.ForceRebuildLayoutImmediate(VTf);
        LayoutRebuilder.ForceRebuildLayoutImmediate(STf);
        LayoutRebuilder.ForceRebuildLayoutImmediate(Tf);
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