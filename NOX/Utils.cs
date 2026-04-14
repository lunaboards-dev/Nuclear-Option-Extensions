using System;
using System.Runtime.InteropServices;
using Steamworks;

namespace NOX;

public class TrackedPtr
{
    IntPtr ptr;
    public IntPtr Ptr => ptr;
    public TrackedPtr(byte[] buffer)
    {
        ptr = Marshal.AllocHGlobal(buffer.Length);
        Marshal.Copy(buffer, 0, ptr, buffer.Length);
    }

    ~TrackedPtr()
    {
        Marshal.FreeHGlobal(ptr);
    }
    
    public static implicit operator IntPtr(TrackedPtr ptr)
    {
        return ptr.ptr;
    }
}

// random bullshit i might use here and there
class Utils
{
    public static TrackedPtr Buffer(byte[] buffer)
    {
        return new TrackedPtr(buffer);
    }

    public static byte[] ConvertFrom<T>(T src) where T : struct
    {
        var src_span = MemoryMarshal.CreateSpan(ref src, 1);
        var val = MemoryMarshal.Cast<T, byte>(src_span);
        return val.ToArray();
    }

    public static T ConvertTo<T>(byte[] src) where T : struct
    {
        var val = MemoryMarshal.Cast<byte, T>(src);
        return val[0];
    }

    public static bool SendSteamMessage(ref SteamNetworkingIdentity ident, int channel, byte[] msg, byte flags = 8)
    {
        var mptr = Buffer(msg);
        return SteamNetworkingMessages.SendMessageToUser(ref ident, mptr, (uint)msg.Length, flags, channel) != EResult.k_EResultOK;
    }
}