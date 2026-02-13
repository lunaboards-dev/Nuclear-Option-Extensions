using System;
using System.Diagnostics;
using System.IO;
using BepInEx;
using BepInEx.Logging;

namespace NOX.Manager;

class SevenZip
{
    static string nix_path = "7za";

    static string[] paths = ["NOX", "NOX/NOX"];
    
    static int RunCommand(string cmd, string args)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = cmd;
        startInfo.Arguments = args;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;

        Process processTemp = new Process();
        processTemp.StartInfo = startInfo;
        processTemp.EnableRaisingEvents = true;
        try
        {
            processTemp.Start();
            processTemp.WaitForExit();
            return processTemp.ExitCode;
        }
        catch (Exception e)
        {
            Plugin.Logger.LogWarning(e.ToString());
            return -1;
        }
    }

    static string seven_path;

    static bool Find7z()
    {
        if (seven_path != null) return true;
        foreach (string s in paths)
        {
            string path = Path.Join(Paths.PluginPath, s, "assets/7zr.exe");
            if (File.Exists(path))
            {
                seven_path = path;
                return true;
            }
        }
        return false;
    }

    public static int Run7z(string args)
    {
        if (!Find7z()) return -1;
        return RunCommand(seven_path, args);
    }

    public static bool CheckFor7z()
    {
        var rtv = Run7z("");
        Plugin.Logger.LogInfo("Exit code: "+rtv);
        return rtv == 0;
    }

    public static bool Unpack(string src, string dst)
    {
        string basepath = Paths.BepInExRootPath;
        return Run7z($"x -o{basepath}/{dst} {basepath}/plugincache/{src}") == 0;
    }
}