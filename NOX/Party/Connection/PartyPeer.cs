using System;
using Steamworks;

namespace NOX.Party.Connection;

public class PartyPeer
{
    SteamNetworkingIdentity ident;
    ulong id;
    byte[] expected_challenge = [];
    byte[] old_challenge = [];
    bool seen_current;
    PartyConnection con;

    internal PartyPeer(ref SteamNetworkingIdentity iden, PartyConnection conn)
    {
        ident = iden;
        con = conn;
        id = iden.GetSteamID64();
    }

    public bool ValidateCurrentChallenge(byte[] chal)
    {
        bool res = chal.SequenceEqual(expected_challenge);
        if (res)
            seen_current = true;
        return res;
    }

    public void UpdateChallenge()
    {
        old_challenge = expected_challenge;
        expected_challenge = con.CalculateCurrentChallenge(id);
    }

    public bool Validate(PartyMessageHeader msg)
    {
        return ValidateCurrentChallenge(msg.challenge) || 
            (msg.challenge.SequenceEqual(old_challenge) && !seen_current);
    }

    public bool SendRaw(Message type, byte[] data)
    {
        return con.SendMessageTo(ident, type, data);
    }
}