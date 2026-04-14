using System.Runtime.InteropServices;

namespace NOX.Party.Connection.Messages;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
struct Kick
{
    public enum KickReason
    {
        Kick = 0,
        Timeout
    }
    public ulong member;
    public KickReason reason;
}