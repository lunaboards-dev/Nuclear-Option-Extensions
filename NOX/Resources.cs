using System.IO;
using BepInEx;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

namespace NOX;

public static class Resources
{
    public static Texture2D RWRBaseTex;
    public static Texture2D RWRContact;
    public static Texture2D RWRContactLo;
    public static Texture2D RWRContactHi;
    public static Font Font;

    public static void Init()
    {
        
        RWRBaseTex = LoadTex("base.png", 300, 300);
        RWRContact = LoadTex("contact.png", 43, 43);
        RWRContactLo = LoadTex("contact_lo.png", 43, 43);
        RWRContactHi = LoadTex("contact_hi.png", 43, 43);

        AllFonts();

        Font = UnityEngine.Resources.Load<Font>("Font/regular.otf");//GetBuiltinResource<Font>("Font/regular_cozy.otf");
        if (Font == null)
        {
            Plugin.Logger.LogError("Font not found!");
            Font = UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
    }

    static Texture2D LoadTex(string path, int w, int h)
    {
        Texture2D tmp = new Texture2D(w, h);
        byte[] data = LoadAsset(path);
        ImageConversion.LoadImage(tmp, data);
        return tmp;
    }

    static byte[] LoadAsset(string path)
    {
        Plugin.Logger.LogInfo($"Loading asset: {path}");
        string iconPath = Path.Combine(Paths.PluginPath, "NOX", "assets", path);
        byte[] rtv =  File.ReadAllBytes(iconPath);
        return rtv;
    }

    public static void AllFonts()
    {
        // ugly hack
        Font fnt = SceneSingleton<HeadMountedDisplay>.i?.gameObject.GetComponentInChildren<HUDApp>()?.gameObject.GetComponentInChildren<Text>().font;
        if (fnt != null)
        {
            Font = fnt;
        }
    }
}