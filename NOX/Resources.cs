using System.IO;
using System.Reflection;
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
    public static Texture2D RWRLineDash;
    public static Font Font;
    public static Font UIFont;

    public static void Init()
    {
        int base_size = 600;
        int contact_size = 86;
        RWRBaseTex = LoadTex("base.png", base_size, base_size);
        RWRContact = LoadTex("contact.png", contact_size, contact_size);
        RWRContactLo = LoadTex("contact_lo.png", contact_size, contact_size);
        RWRContactHi = LoadTex("contact_hi.png", contact_size, contact_size);
        RWRLineDash = LoadTex("line_dash.png", 1, 30);
        RWRLineDash.wrapMode = TextureWrapMode.Repeat;

        /*foreach (var F in assembly.GetManifestResourceNames())
        {
            Plugin.Logger.LogInfo(F);
        }*/

        AllFonts();

        /*Font = UnityEngine.Resources.Load<Font>("Font/regular.otf");//GetBuiltinResource<Font>("Font/regular_cozy.otf");
        if (Font == null)
        {
            Plugin.Logger.LogError("Font not found!");*/
        Font = UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf"); // temporary until the actual font is loaded
        UIFont = Font;
        //}
        BetaTesters.LoadBetaTesters();
    }

    static Assembly assembly = Assembly.GetExecutingAssembly();

    static Texture2D LoadTex(string path, int w, int h)
    {
        Texture2D tmp = new Texture2D(w, h);
        byte[] data = Plugin.LoadExternal.Value ? LoadAsset(path) : LoadAssetInternal(path);
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

    static byte[] LoadAssetInternal(string path)
    {
        Plugin.Logger.LogInfo($"Loading internal asset: {path}");
        using (Stream stream = assembly.GetManifestResourceStream("NOX.assets."+path))
        {
            if (stream == null) return [];
            //Plugin.Logger.LogInfo($"Loading internal asset (real): {path}");
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);

            return buffer;
        }
    }

    // THIS USES THE FULL PATH
    public static Stream GetStream(string path) => assembly.GetManifestResourceStream(path);

    public static string[] GetResourceNames() => assembly.GetManifestResourceNames();

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