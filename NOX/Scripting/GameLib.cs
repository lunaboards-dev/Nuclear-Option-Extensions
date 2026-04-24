using System;
using KeraLua;
using UnityEngine;

namespace NOX.Scripting;

public class GameLib
{
    [LuaCall("game", "leveltime")]
    public static int lua_leveltime(ScriptEnv L)
    {
        L.PushNumber(Time.timeSinceLevelLoad);
        return 0;
    }

    [LuaCall("log", "info")]
    public static int lua_loginfo(ScriptEnv L)
    {
        string msg = L.CheckString(1);
        Plugin.LuaLogger.LogInfo(msg);
        return 0;
    }

    [LuaCall("log", "error")]
    public static int lua_logerror(ScriptEnv L)
    {
        string msg = L.CheckString(1);
        Plugin.LuaLogger.LogError(msg);
        return 0;
    }

    [LuaCall("log", "debug")]
    public static int lua_logdebug(ScriptEnv L)
    {
        string msg = L.CheckString(1);
        Plugin.LuaLogger.LogDebug(msg);
        return 0;
    }

    [LuaCall("log", "fatal")]
    public static int lua_logfatal(ScriptEnv L)
    {
        string msg = L.CheckString(1);
        Plugin.LuaLogger.LogFatal(msg);
        return 0;
    }
}