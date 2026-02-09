using System;
using UnityEngine;
using UnityEngine.Networking;

namespace NOX.Manager;

#pragma warning disable CS0649

[Serializable]
struct NOMArtifact
{
    public string fileName;
    public string version;
    public string category;
    public string type;
    public string gameVersion;
    public string downloadUrl;
    public string hash;
    public (string id, string version)[] dependencies;
}

[Serializable]
struct NOMMod
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
}

[Serializable]
struct NOMRepo
{
    public NOMMod[] mods;
}

#pragma warning restore CS0649

class NOM
{
    NOMRepo repo;
    UnityWebRequest www;

    public Action<NOM> Ready;
    public Action<NOM> Failure;

    internal NOM()
    {
        www = UnityWebRequest.Get("https://raw.githubusercontent.com/KopterBuzz/NOModManifestTesting/refs/heads/main/manifest/manifest.json");
        var req = www.SendWebRequest();
        req.completed += OnComplete;
    }

    void OnComplete(AsyncOperation op)
    {
        if (www.result != UnityWebRequest.Result.Success)
        {
            Failure(this);
        }
        string res = "{\"mods\": "+www.downloadHandler.text+"}";
        repo = JsonUtility.FromJson<NOMRepo>(res);
        Ready(this);
    }
}