using System.Numerics;
using NOX.Scripting;
using NuclearOption;
using UnityEngine;
using UnityEngine.UI;

namespace NOX.RWRs.Script;

public class RWRLight : MonoBehaviour
{
    GameObject TextHolder;
    Text text;
    Image image;

    void Push(ScriptEnv L)
    {
        L.PushObject(this);
        L.SetMetaTable("RWRLight");
    }

    // position(x, y)
    [LuaCall("RWRLight", "position", true)]
    public static int lua_cpos(ScriptEnv L)
    {
        var light = L.CheckObject<RWRLight>(1, "RWRLight");
        var x = L.CheckNumber(2);
        var y = L.CheckNumber(3);
        light.transform.AsRectTransform().anchoredPosition = new UnityEngine.Vector2((float)x, (float)y);
        return 0;
    }

    // label(text) or label()
    [LuaCall("RWRLight", "label", true)]
    public static int lua_text(ScriptEnv L)
    {
        var light = L.CheckObject<RWRLight>(1, "RWRLight");
        if (L.GetTop() == 2)
        {
            light.text.text = L.CheckString(2);
        }
        return 0;
    }

    // color(r, g, b, a)
    [LuaCall("RWRLight", "color", true)]
    public static int lua_color(ScriptEnv L)
    {
        var light = L.CheckObject<RWRLight>(1, "RWRLight");
        var r = L.CheckNumber(2);
        var g = L.CheckNumber(3);
        var b = L.CheckNumber(4);
        var a = L.CheckNumber(5);
        var color = new Color((float)r, (float)g, (float)b, (float)a);
        light.image.color = color;
        light.text.color = color;
        return 0;
    }

    // destroy()
    /*[LuaCall("RWRLight", "destroy", true)]
    public static int lua_destroy(ScriptEnv L)
    {
        return 0;
    }*/
}