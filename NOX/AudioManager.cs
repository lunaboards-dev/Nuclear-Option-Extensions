// this just finds all audio files in the plugin directory
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using UnityEngine;
using UnityEngine.Networking;

namespace NOX;

class AudioManager
{
    static Dictionary<string, AudioType> supported_ext = new()
    {
        {".mp3", AudioType.MPEG},
        {".wav", AudioType.WAV},
        {".ogg", AudioType.OGGVORBIS}
    };
    static int Loading = 0;
    public static Dictionary<string, AudioClip> Clips = [];
    static internal void RecurseSearchForAudio(string path)
    {
        foreach (string f in Directory.GetFileSystemEntries(path))
        {
            string jpath = f;
            if (Directory.Exists(jpath))
            {
                RecurseSearchForAudio(jpath);
            } else if (File.Exists(f)) {
                string ext = Path.GetExtension(jpath);
                if (supported_ext.ContainsKey(ext)) {
                    LoadAudioClip(jpath, supported_ext[ext]);
                }
            }
        }
    }

    static internal bool Ready => Loading == 0;

    static internal void Build()
    {
        RecurseSearchForAudio(Paths.PluginPath);
    }

    static internal AudioClip Lookup(string s)
    {
        if (!Ready) return null;
        foreach (var pair in Clips)
        {
            if (pair.Key.EndsWith(s))
            {
                return pair.Value;
            }
        }
        Plugin.Logger.LogError("can't find sound "+s);
        return null;
    }

    static void LoadAudioClip(string path, AudioType t)
    {
        string fpath = "file://"+path;
        var www = UnityWebRequestMultimedia.GetAudioClip(fpath, t);
        Plugin.Logger.LogDebug("Loading sound "+fpath);
        Loading++;
        www.SendWebRequest().completed += (op) =>
        {
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Plugin.Logger.LogError(www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                string cpath = path.Replace('\\', '/');
                Clips.Add(cpath, clip);
                Plugin.Logger.LogDebug("Loaded sound "+cpath);
            }
            Loading--;
            if (Loading == 0)
            {
                //idk do something
                Plugin.Logger.LogInfo("All audio loaded.");
                Resources.LoadCrazy();
            }
        };
    }
}