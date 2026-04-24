using System.Collections.Generic;
using System.IO;
using KeraLua;
using NOX.Scripting;
using UnityEngine;

namespace NOX.RWRs;
public class ScriptableRWR : IRWRDisplay
{
    string BasePath;
    Dictionary<string, AudioClip[]> Clips = [];
    //Dictionary<string, GameObject> Objects = [];
    List<GameObject> Objects = [];
    string name;
    bool supports_filters;
    static ScriptEnv L;
    AudioSource MAWSource;

    static void Setup()
    {
        L = new ScriptEnv("rwr");
        L.OpenLibs();
        L.PushCSFunction((L) =>
        {
            string path = L.CheckString(1);
            if (L.LoadInternalScript("NOX.lua."+path+".lua") == LuaStatus.OK)
            {
                if (L.PCall(1, 1, 0) == LuaStatus.OK)
                {
                    return 1;
                }
            }
            return 0;
        });
        L.SetGlobal("include");
    }

    public ScriptableRWR(string path)
    {
        L.PushLightUserData((nint)GetHashCode());
        L.NewTable();

        L.PushString("maws_sound");
        L.PushCSFunction((L) =>
        {
            string seeker = L.CheckString(1);
            L.CheckType(2, LuaType.Table);
            int count = (int)L.Length(2);
            AudioClip[] clips = new AudioClip[count];
            for (int i=0;i<count;++i)
            {
                L.PushInteger(i+1);
                L.GetTable(2);
                string path = L.ToString(-1);
                clips[i] = AudioManager.Lookup(path);
                L.Pop(1);
            }
            return 0;
        });
        L.SetTable(-3);

        L.PushString("create_element");
        L.PushCSFunction((L) =>
        {
            
            return 0;
        });
        L.SetTable(-3);

        L.PushCopy(-1);
        L.SetGlobal("RWR");
        L.SetTable((int)LuaRegistry.Index);
        L.LoadInternalScript("NOX.assets.lua.rwrs."+path+".lua");
        BasePath = path;
    }

    public void PrepareFunction(string name)
    {
        L.SetTop(0);
        L.PushLightUserData((nint)GetHashCode());
        L.GetTable((int)LuaRegistry.Index);
        L.PushString(name);
        L.GetTable(-2);
        L.PushCopy(-2);
    }

    public bool SupportFilters => throw new System.NotImplementedException();

    public string SystemName => throw new System.NotImplementedException();

    public void Init()
    {
        PrepareFunction("init");
        L.PCall(1,0,0);
    }

    public void Update()
    {
        PrepareFunction("update");
        L.PCall(1,0,0);
    }

    public void Contact(RWRContact contact)
    {
        PrepareFunction("contact");
        contact.Push(L);
        L.PCall(2,0,0);
    }

    public void Destroy()
    {
        PrepareFunction("destroy");
        if (L.Type(-2) == LuaType.Function)
            L.PCall(1,0,0);
    }
}