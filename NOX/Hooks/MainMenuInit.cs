using HarmonyLib;
using NOX.UI.ModManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NOX.Hooks;

[HarmonyPatch(typeof(MainMenu), "Awake")]
class MainMenuInit
{
    public static TextMeshProUGUI label;
    public static MainMenu menu;
    static void Postfix(MainMenu __instance)
    {
        if (!BetaTesters.LocalIsTester) return;
        menu = __instance;
        var m_menu = __instance.gameObject;

        var _leftpanel = menu.GetChildByName("LeftPanel");
        var _container = _leftpanel.GetChildByName("Container");

        var panel = _container.GetChildByName("MenuButtonsPanel");

        var button = GameObject.Instantiate(panel.GetChildByName("WorkshopButton"));
        button.name = "NOXModManagerButton";
        button.transform.SetParent(panel.transform, false);

        var _label = button.GetChildByName("WorkshopButtonLabel");
        _label.name = "NOXModManagerLabel";
        label = _label.GetComponent<TextMeshProUGUI>();
        label.text = "MODS";

        foreach (var font in UnityEngine.Resources.FindObjectsOfTypeAll<Font>())
        {
            if (font.name == "regular")
                Resources.RegularFont = font;
        }
    }
}