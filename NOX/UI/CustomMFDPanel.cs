using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Mono.Cecil;
using NOX.Hooks;
using UnityEngine;
using UnityEngine.UI;

namespace NOX.UI;

abstract class CustomMFDPanel
{
    public static List<GameObject> panels = [];
    public abstract string Label {get;}
    public abstract string Title {get;}
    public GameObject PanelInner => panel_inner;
    private GameObject panel_inner;
    private Text label;
    public Text LabelObject => label;
    internal void InitPanel(GameObject go)
    {
        
    }
    public abstract void PanelCreate();
    public abstract void PanelUpdate();
    public abstract void PanelDestroy();
    public static void Register<T>(VirtualMFD mfd) where T : CustomMFDPanel, new()
    {
        var go = new GameObject("NOX_MFDPanel");
        var pan = go.AddComponent<CustomMFDPaneBehavior>();
        pan.SetPanel<T>();
        panels.Add(go);
    }

    public sealed class CustomMFDPaneBehavior : MonoBehaviour
    {
        CustomMFDPanel instance;
        GameObject panel;
        GameObject inner;
        MFDScreen app;
        ContentSizeFitter fitter;
        internal bool left_side;
        GameObject title;
        VirtualMFD mfd;
        Button button;
        RectTransform transform;
        GameObject layout;
        VerticalLayoutGroup group;

        private static readonly FieldInfo right = AccessTools.Field(typeof(VirtualMFD), "rightScreens");
        private static readonly FieldInfo left = AccessTools.Field(typeof(VirtualMFD), "leftScreens");
        private static readonly FieldInfo bright = AccessTools.Field(typeof(VirtualMFD), "rightButtons");
        private static readonly FieldInfo bleft = AccessTools.Field(typeof(VirtualMFD), "leftButtons");

        public bool FindFirstFree(MFDScreen screen)
        {
            mfd = FindObjectOfType<VirtualMFD>();
            var mfdobj = mfd.gameObject;
            if (mfd != null) {
                screen.gameObject.transform.SetParent(mfdobj.transform, false);
                if (bright.GetValue(mfd) is List<Button> rightButtons &&
                    bleft.GetValue(mfd) is List<Button> leftButtons &&
                    right.GetValue(mfd) is List<MFDScreen> rightScreens &&
                    left.GetValue(mfd) is List<MFDScreen> leftScreens)
                {
                    var fright = rightScreens.IndexOf(null);
                    var fleft = leftScreens.IndexOf(null);
                    if (fright > -1)
                    {
                        rightScreens[fright] = screen;
                        screen.label = rightButtons[fright].gameObject.GetChildComponentByName<Text>("Label");
                        screen.highlight = rightButtons[fright].gameObject.GetChildComponentByName<Image>("Highlight");
                        button = rightButtons[fright];
                        button.interactable = true;
                        button.enabled = true;
                        //rightButtons[fright].onClick.AddListener(ButtonClickRight);
                    } else if (fleft > -1)
                    {
                        leftScreens[fleft] = screen;
                        screen.label = leftButtons[fleft].gameObject.GetChildComponentByName<Text>("Label");
                        screen.highlight = rightButtons[fright].gameObject.GetChildComponentByName<Image>("Highlight");
                        button = leftButtons[fleft];
                        button.interactable = true;
                        button.enabled = true;
                        //leftButtons[fleft].onClick.AddListener(ButtonClickLeft);
                    } else
                    {
                        throw new OverflowException("unable to add MFD panel");
                    }
                    return true;
                } else {
                    throw new MemberNotFoundException("unable to find MFD buttons");
                }
            }
            throw new MemberNotFoundException("unable to find MFD");
        }

        void Awake()
        {
            panel = gameObject;
            var img = panel.AddComponent<Image>();
            img.sprite = RWRMFDButton.sprite;
            app =  panel.AddComponent<MFDScreen>();
            fitter = panel.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
            app.displayPanel = panel;
            app.aircraftOnly = true;
        }

        internal void SetPanel<T>() where T : CustomMFDPanel, new()
        {
            instance = new T();
            inner = new GameObject("NOXInnerPanel");
            title = new GameObject("NOXPanelTitle");
            var title_txt = title.AddComponent<Text>();
            var title_tf = title.GetComponent<RectTransform>();
            title_tf.anchorMax = new Vector2(0.5f, 1);
            title_tf.anchorMin = new Vector2(0.5f, 1);
            title_tf.pivot = new Vector2(0.5f, 1);
            app.shortName = instance.Label;
            FindFirstFree(app);
            instance.panel_inner = inner;
            instance.label = app.label;
            title_txt.text = instance.Title;
            title_txt.font = Resources.UIFont;
            title_txt.fontSize = 30;
            //app.label.text = instance.Label;
            //instance.label.text = instance.Label;
            instance.InitPanel(panel);
            instance.PanelCreate();
            Plugin.Logger.LogDebug("Panel created!");
        }

        void Update()
        {
            instance.PanelUpdate();
        }

        void OnDestroy()
        {
            instance.PanelDestroy();
        }
    }
}