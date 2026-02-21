using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using NOX.Hooks;
using UnityEngine;
using UnityEngine.UI;

namespace NOX.UI;

abstract class CustomMFDPanel
{
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
        go.AddComponent<CustomMFDPaneBehavior<T>>();
    }

    public sealed class CustomMFDPaneBehavior<T> : MonoBehaviour where T : CustomMFDPanel, new()
    {
        T instance;
        GameObject panel;
        GameObject inner;
        MFDScreen app;
        ContentSizeFitter fitter;
        internal bool left_side;

        private static readonly FieldInfo right = AccessTools.Field(typeof(VirtualMFD), "rightScreens");
        private static readonly FieldInfo left = AccessTools.Field(typeof(VirtualMFD), "leftScreens");
        private static readonly FieldInfo bright = AccessTools.Field(typeof(VirtualMFD), "rightButtons");
        private static readonly FieldInfo bleft = AccessTools.Field(typeof(VirtualMFD), "leftButtons");

        public bool FindFirstFree(MFDScreen screen)
        {
            var mfd = FindObjectOfType<VirtualMFD>();
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
                        rightScreens.Add(screen);
                        screen.label = rightButtons[fright].gameObject.GetChildComponentByName<Text>("Label");
                        screen.highlight = rightButtons[fright].gameObject.GetChildComponentByName<Image>("Highlight");
                    }
                    if (fleft > -1)
                    {
                        leftScreens.Add(screen);
                        screen.label = leftButtons[fleft].gameObject.GetChildComponentByName<Text>("Label");
                        screen.highlight = rightButtons[fright].gameObject.GetChildComponentByName<Image>("Highlight");
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
            inner = new GameObject("NOXInnerPanel");
            instance = new T();
            app.shortName = instance.Label;
            FindFirstFree(app);
            instance.panel_inner = inner;
            instance.label = app.label;
            //instance.label.text = instance.Label;
            instance.InitPanel(panel);
            instance.PanelCreate();
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