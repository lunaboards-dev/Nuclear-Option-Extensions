using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using KeraLua;

namespace NOX.Scripting;

public class ScriptEnv : Lua
{
    public delegate int ScriptFunction(ScriptEnv L);
    private class ScriptLib
    {
        public string Name;
        public string Context;
        public bool Metatable;
        public ScriptLib(string n, string c, bool m)
        {
            Name = n;
            Context = c;
            Metatable = m;
            Plugin.Logger.LogDebug($"New library: {n} ({c})");
        }
        Dictionary<string, LuaFunction> Funcs = [];

        public void AddCall(string name, ScriptFunction fun)
        {
            Funcs.Add(name, (Lptr) =>
            {
                ScriptEnv env = (ScriptEnv)FromIntPtr(Lptr);
                return fun(env);
            });
        }

        public void LoadLib(ScriptEnv env)
        {
            if (Metatable)
            {
                env.NewMetaTable(Name);
                env.PushString("__index");
            }
            env.NewTable();
            
            foreach (var pair in Funcs)
            {
                env.PushString(pair.Key);
                env.PushCFunction(pair.Value);
                env.SetTable(-3);
            }

            if (Metatable)
            {
                env.SetTable(-3);
                env.Pop(1);
            } else
                env.SetGlobal(Name);
        }
    }

    private static List<ScriptLib> Libs = [];
    internal static void Setup()
    {
        var ass = Assembly.GetExecutingAssembly();
        var funcs = from type in ass.GetTypes()
                    from method in type.GetMethods()
                    where method.IsDefined(typeof(LuaCallAttribute)) && method.IsStatic
                    select method;
        foreach (var func in funcs)
        {
            LuaCallAttribute lattr = (LuaCallAttribute) Attribute.GetCustomAttribute(func, typeof(LuaCallAttribute));
            ScriptLib lib;
            foreach (var l in Libs)
            {
                if (l.Name == lattr.lib)
                {
                    lib = l;
                    goto found;
                }
            }
            lib = new ScriptLib(lattr.lib, lattr.ctx);
            Libs.Add(lib);
            found:
            lib.AddCall(lattr.name, (ScriptFunction) func.CreateDelegate(typeof(ScriptFunction)));
        }
        Plugin.Logger.LogInfo($"Lua environments ready with {Libs.Count} libraries loaded.");
    }
    public ScriptEnv(string context) : base()
    {
        OpenLibs();
        foreach (var l in Libs)
        {
            if (l.Context == null || l.Context == context)
            {
                l.LoadLib(this);
            }
        }
    }

    public LuaStatus LoadInternalScript(string path)
    {
        using var stream = Resources.GetStream(path);
        using var reader = new StreamReader(stream);
        string src = reader.ReadToEnd();
        return LoadString(src, path);
    }

    public void PushCSFunction(ScriptFunction func)
    {
        PushCFunction((Lptr) =>
        {
            ScriptEnv env = (ScriptEnv)FromIntPtr(Lptr);
            return func(env);
        });
    }
}