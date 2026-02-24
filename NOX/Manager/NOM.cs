using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Bootstrap;
using Newtonsoft.Json;
using NOX.Hooks;
using NOX.UI.ModManager;
using UnityEngine;
using UnityEngine.Networking;

namespace NOX.Manager;

#pragma warning disable CS0649

[Serializable]
class NOMArtifact
{
    public string fileName;
    public string version;
    public string category;
    public string type;
    public string gameVersion;
    public string downloadUrl;
    public string hash;
    public (string id, string version)[] dependencies;
    public Version AddonVersion => new Version(version);
}

[Serializable]
class NOMMod
{
    public string id;
    public string displayName;
    public string description;
    public string[] tags;
    public (string name, string url)[] urls;
    public string[] authors;
    public string githubOwner;
    public string githubRepoName;
    public string autoUpdateArtifacts;
    public NOMArtifact[] artifacts;
    public int downloadCount;
    public IEnumerable<NOMArtifact> StableArtifacts => artifacts.Where(art=>art.category == "release");

    static NOMArtifact Latest(IEnumerable<NOMArtifact> list)
    {
        NOMArtifact latest = null;
        foreach (var art in list)
        {
            if (latest == null || art.AddonVersion > latest.AddonVersion)
                latest = art;
        }
        return latest;
    }
    public NOMArtifact LatestStable => Latest(StableArtifacts);
    public NOMArtifact LatestArtifact => Latest(artifacts);
}

[Serializable]
struct NOMRepo
{
    public NOMMod[] mods;
}

#pragma warning restore CS0649

public class NOM
{
    //NOMRepo repo;
    NOMMod[] mods;
    UnityWebRequest www;

    public Action<NOM> Ready;
    public Action<NOM> Failure;

    internal NOM()
    {
        Plugin.Logger.LogInfo("Refreshing NOM repo...");
        www = UnityWebRequest.Get("https://kopterbuzz.github.io/NOMNOM/manifest/manifest.json");
        var req = www.SendWebRequest();
        req.completed += OnComplete;
    }

    void OnComplete(AsyncOperation op)
    {
        if (www.result != UnityWebRequest.Result.Success)
        {
            Failure(this);
        }
        //string res = "{\"mods\": "+www.downloadHandler.text+"}";
        //repo = JsonConvert.DeserializeObject<NOMRepo>(res);
        mods = JsonConvert.DeserializeObject<NOMMod[]>(www.downloadHandler.text);
        UpdateCheck();
        Ready?.Invoke(this);
        Plugin.Logger.LogInfo($"NOM repo refreshed. {mods.Length} mods.");
    }

    void DownloadMod(string package, string version, string path)
    {
        foreach (var mod in mods)
        {
            if (mod.id == package)
            {
                foreach (var artifact in mod.artifacts) {
                    
                    var req = UnityWebRequest.Get("");
                }
            }
        }
    }

    void UnpackMod(string path, string result)
    {
        
    }

    NOMMod GetMod(string id) => mods.Where(mod => mod.id == id).FirstOrDefault();

    void UpdateCheck()
    {
        int updates = 0;
        foreach (var plug_info in Chainloader.PluginInfos)
        {
            var info = plug_info.Value.Metadata;
            var modinfo = GetMod(info.GUID);
            if (modinfo != null)
            {
                var latest = modinfo.LatestStable;
                if (latest.AddonVersion > info.Version)
                {
                    updates++;
                    Plugin.Logger.LogWarning($"Update available for {info.Name} (Latest: {latest.AddonVersion}, current: {info.Version})");
                }
            }
        }
        MainMenuInit.label.text += $" ({updates})";
        var mi = Extensions.CreateObjectWithComponent<ModInfo>("test");
        mi.SetMod(mods[0]);
        mi.transform.SetParent(MainMenuInit.menu.transform, false);
    }
}