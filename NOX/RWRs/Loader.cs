using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Analytics;

namespace NOX.RWRs;

class Loader
{
    #pragma warning disable CS0649
    [Serializable]
    public class NOXRadar
    {
        public string band;
        public string type;
        public string id;
    }

    [Serializable]
    public class NOXRWRDef
    {
        public string type;
        public List<string> mask;
        public bool elevation;
    }

    [Serializable]
    public class NOXUnitDef
    {
        public int version; // should always be 0
        public string deftype; // should always be "unit"
        public string name;
        [SerializeField]
        public NOXRadar threat;
        [SerializeField]
        public NOXRWRDef rwr;
    }
    #pragma warning restore CS0649

    static Dictionary<string, RWRThreatType> Mapping = new()
    {
        {"air intercept", RWRThreatType.AirIntercept},
        {"attacker", RWRThreatType.Attacker},
        {"aew", RWRThreatType.AEW},
        {"sam", RWRThreatType.SAM},
        {"aaa", RWRThreatType.AAA},
        {"naval", RWRThreatType.Ship},
        {"fcs", RWRThreatType.FCS},
        {"early warning", RWRThreatType.EarlyWarning}
    };

    static Dictionary<string, IRWRSystem> SystemMap = new()
    {
        {"band", RWR.Band},
        {"helicopter", RWR.Helicopter},
        {"fighter", RWR.Fighter},
        {"full", RWR.Full}
    };

    static RWRThreatType GetThreat(string path, string threat)
    {
        if (Mapping.TryGetValue(threat, out RWRThreatType t))
        {
            return t;
        } else
        {
            Plugin.Logger.LogWarning($"DefLoader warning ({path}): Invalid threat type.");
            return 0;
        }
    }

    static void LoadFile(string path)
    {
        using var stream = File.OpenRead(path);
        LoadFile(stream, path);
    }

    static void LoadInternal(string path)
    {
        using var stream = Resources.GetStream(path);
        LoadFile(stream, "DLL:"+path);
    }

    static void LoadFile(Stream stream, string path)
    {
        Plugin.Logger.LogInfo($"DefLoader loading file {path}");
        //string str = File.ReadAllText(path);
        StreamReader reader = new(stream);
        string str = reader.ReadToEnd();
        reader.Close();
        var def = JsonConvert.DeserializeObject<NOXUnitDef>(str);//JsonUtility.FromJson<NOXUnitDef>(str);
        //Plugin.Logger.LogInfo($"DefLoader ({path}): rwr = {def.rwr}, threat = {def.threat}");
        if (def.version != 0 || def.deftype != "unit")
        {
            Plugin.Logger.LogError($"DefLoader error ({path}): invalid version or deftype. Def not loaded.");
            return;
        }
        if (def.name == null)
        {
            Plugin.Logger.LogError($"DefLoader error ({path}): unit name is null. Def not loaded.");
        }
        if (def.rwr != null)
        {
            NOXRWRDef rwr = (NOXRWRDef) def.rwr;
            if (rwr.type == "mask")
            {
                if (rwr.mask == null)
                {
                    Plugin.Logger.LogWarning($"DefLoader warning ({path}): Mask RWR with no mask.");
                    rwr.mask = [];
                }
                RWRThreatType mask = 0;
                foreach (var m in rwr.mask)
                {
                    mask |= GetThreat(path, m);
                }
                IRWRSystem masked = new Systems.Mask(rwr.elevation, mask);
                RWR.AddRWR(def.name, masked);
            } else if (rwr.type != null && SystemMap.TryGetValue(rwr.type, out IRWRSystem system))
            {
                RWR.AddRWR(def.name, system);
            } else
            {
                Plugin.Logger.LogError($"DefLoader error ({path}): Unknown RWR type {rwr.type}");
            }
            Plugin.Logger.LogDebug($"DefLoader loaded RWR for unit {def.name}: {rwr.type}");
        }
        if (def.threat != null)
        {
            NOXRadar threat = (NOXRadar) def.threat;
            if (threat.band == null || threat.band.Length != 1)
            {
                Plugin.Logger.LogError($"DefLoader error ({path}): invalid radar band. Threat not added."); goto cont;
            }
            if (threat.type == null || !Mapping.ContainsKey(threat.type))
            {
                Plugin.Logger.LogError($"DefLoader error ({path}): invalid radar type. Threat not added."); goto cont;
            }
            if (threat.id == null || threat.id.Length < 1)
            {
                Plugin.Logger.LogError($"DefLoader error ({path}): invalid threat ID. Threat not added."); goto cont;
            }
            //Plugin.Logger.LogInfo($"DefLoader ({path}): Added radar (name = {def.name}, id= {threat.id}, type = {threat.type}, band = {threat.band})");
            Threats.RegisterThreat(new RWRThreatID()
            {
                Band = threat.band,
                Class = GetThreat(path, threat.type),
                Display = threat.id,
                Name = def.name
            });
            Plugin.Logger.LogDebug($"DefLoader loaded theat ID for unit {def.name}");
        }
        cont:;
    }

    static internal void RecurseSearchForConfigs(string path)
    {
        foreach (string f in Directory.GetFileSystemEntries(path))
        {
            string jpath = f;
            if (Directory.Exists(jpath))
            {
                RecurseSearchForConfigs(jpath);
            } else if (File.Exists(f)) {
                LoadFile(jpath);
            }
        }
    }

    static internal void RecurseSearchForConfigDir(string path)
    {
        //Plugin.Logger.LogDebug("Searching path: "+path);
        foreach (string f in Directory.GetDirectories(path))
        {
            string jpath = f;
            if (Directory.Exists(jpath))
            {
                //Plugin.Logger.LogDebug("> Searching path: "+jpath);
                if (Path.GetFileName(f) == ".noxcfg")
                {
                    Plugin.Logger.LogDebug("Found config directory: "+jpath);
                    RecurseSearchForConfigs(jpath);
                } else
                {
                    RecurseSearchForConfigDir(jpath);
                }
            }
        }
    }

    static internal void LoadNOXConfigs()
    {
        foreach (string rsc in Resources.GetResourceNames())
        {
            if (rsc.StartsWith("NOX.assets.units."))
                LoadInternal(rsc);
        }
        RecurseSearchForConfigDir(Paths.PluginPath);
    }
}