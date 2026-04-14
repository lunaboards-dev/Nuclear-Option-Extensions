using System.Drawing;
using System.Runtime.InteropServices;

namespace NOX.Party.Connection;

public struct PartyMessageHeader
{
    public Message id;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public byte[] challenge;
}