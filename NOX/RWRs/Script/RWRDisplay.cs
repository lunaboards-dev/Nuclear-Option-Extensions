using NOX.Scripting;
using UnityEngine;

namespace NOX.RWRs.Script;

public class RWRDisplay : MonoBehaviour
{
    
    [LuaCall("RWRDisplay", "contact", true)]
    public static int lua_newcontact(ScriptEnv L)
    {
        var display = L.CheckObject<RWRDisplay>(1, "RWRDisplay");
        return 0;
    }

    // position(angle, distance, elevation)
    [LuaCall("RWRContact", "position", true)]
    public static int lua_cpos(ScriptEnv L)
    {
        return 0;
    }

    // label(text) or label()
    [LuaCall("RWRContact", "label", true)]
    public static int lua_text(ScriptEnv L)
    {
        return 0;
    }

    // update()
    [LuaCall("RWRContact", "update", true)]
    public static int lua_update(ScriptEnv L)
    {
        return 0;
    }

    // destroy()
    [LuaCall("RWRContact", "destroy", true)]
    public static int lua_destroy(ScriptEnv L)
    {
        return 0;
    }
}