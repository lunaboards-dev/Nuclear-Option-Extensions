using System;

namespace NOX.Scripting;

public class LuaCallAttribute : Attribute
{
    public string lib;
    public string name;
    public string ctx;
    internal bool meta;

    public LuaCallAttribute(string v1, string v2)
    {
        lib = v1;
        name = v2;
    }
    public LuaCallAttribute(string v1, string v2, string v3)
    {
        lib = v1;
        name = v2;
        ctx = v3;
    }

    public LuaCallAttribute(string v1, string v2, bool v3)
    {
        lib = v1;
        name = v2;
        meta = v3;
    }
}