using System;
using System.Runtime.InteropServices;

namespace NOX.Manager;

class WineDetect
{
    [DllImport("ntdll.dll")]
    public static extern string wine_get_version();

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
    [return: MarshalAs(UnmanagedType.LPStr)]
    public static extern string wine_get_unix_file_name([MarshalAs(UnmanagedType.LPWStr)] string dos);

    public static bool OnWine = false;

    public static void detect()
    {
        try
        {
            OnWine = wine_get_version() != null;
        } catch (Exception)
        {
            // ignore it
        }
    }

    public static bool is_wine()
    {
        return OnWine;//return wine_get_version() != null;
    }
}