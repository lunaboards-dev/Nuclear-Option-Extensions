using System;
using KeraLua;

public class GameLib
{
    [LuaCall("game", "leveltime")]
    public static int lua_leveltime(Lua L)
    {
        return 0;
    }
}

internal class LuaCallAttribute : Attribute
{
    private string lib;
    private string name;

    public LuaCallAttribute(string v1, string v2)
    {
        
    }
}