using System.IO;
using BepInEx;
using UnityEngine;
using UnityEngine.UI;

namespace NOX.UI;

class RWRDisplayApp : HUDApp
{
    GameObject target;
    public static RWRDisplay Instance;// = new();

    public override void Initialize(Aircraft aircraft)
    {
        target = new GameObject("NOX_RWRDisplay");
        //target = gameObject;
        CombatHUD hud = SceneSingleton<CombatHUD>.i;
        //target.transform.parent = hud.transform;
        //target.transform.SetParent(hud.iconLayer);
        /*Canvas = target.AddComponent<Canvas>();
        CanvasRender = target.AddComponent<CanvasRenderer>();
        Canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        LineDrawer.transform.SetParent(Canvas.transform);
        string iconPath = Path.Combine(Paths.PluginPath, "NOX", "assets/rwr_217x217.png");
        byte[] fileData = File.ReadAllBytes(iconPath);
        Texture2D rwr_tex = new Texture2D(256, 256);
        ImageConversion.LoadImage(rwr_tex, fileData);
        RWRBase = new Material(Shader.Find("UI/Default"));
        RWRBase.mainTexture = rwr_tex;
        base.Initialize(aircraft);
        type = AppType.HUD;
        target.SetActive(true);
        //DrawMaterial(217, 217, RWRBase);*/
        HeadMountedDisplay disp = SceneSingleton<HeadMountedDisplay>.i;
        Instance = target.AddComponent<RWRDisplay>();
        Instance.transform.SetParent(disp.transform);
        gameObject.SetActive(true);
    }

    //private int[] newTriangles = [0, 1, 2, 0, 2, 3];

    /*void DrawMaterial(int x, int y, int w, int  h, )
    {
        Mesh tmp = new Mesh();
        tmp.vertices = [
            new Vector3(0, 0),
            new Vector3(0, y, 0),
            new Vector3(x, y, 0),
            new Vector3(x, 0, 0)
        ];
        tmp.triangles = newTriangles;

        CanvasRender.SetMaterial(RWRBase, 0);
        CanvasRender.SetMesh(tmp);
    }*/

    public override void Refresh()
    {
        
    }


}