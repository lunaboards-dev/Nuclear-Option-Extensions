using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Steamworks;
using UnityEngine.UIElements.UIR;

namespace NOX.Party.Connection;

class PartyConnection
{
    static readonly ushort PARTY_CHANNEL = 0xF0EE;

    bool isHost;
    SteamNetworkingIdentity host;
    List<PartyPeer> peers = [];
    int channel;
    double connectionTime;
    byte[] currentChallenge = new byte[32];
    SHA256 sha;
    ulong steamid;
    byte[] myChallenge = new byte[32];
    
    enum ConnectionState
    {
        Connecting,
        AwaitingChallenge,
        Disconnected,
        Connected,
        Raffle,
        Host
    }

    ConnectionState state = ConnectionState.Disconnected;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct ChatMessage
    {
        public string msg;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct HostRequest
    {
        public double join_time;
    }

    enum LobbyStatus
    {
        InGame = 1,
        InviteOnly = 2
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct PartyInfo
    {
        LobbyStatus status;
        byte players;
        ulong host;
    }

    static SteamNetworkingIdentity CreateIdent(ulong id)
    {
        SteamNetworkingIdentity ident = new();
        ident.SetSteamID64(id);
        return ident;
    }

    internal PartyConnection(int chan)
    {
        channel = chan;
        host = new SteamNetworkingIdentity();
        host.SetLocalHost();
        isHost = true;
        sha = SHA256.Create();
        state = ConnectionState.Host;
        steamid = SteamUser.GetSteamID().m_SteamID;
    }

    private PartyConnection(int chan, ulong host)
    {
        var hident = CreateIdent(host);
        channel = chan;
        this.host = hident;
        //SendMessage(Message.JoinRequest, []);
        SendMessageTo(hident, Message.JoinRequest, []);
        sha = SHA256.Create();
        state = ConnectionState.Connecting;
        steamid = SteamUser.GetSteamID().m_SteamID;
    }

    public static async UniTask<PartyConnection> Connect(int chan, ulong host)
    {
        var con = new PartyConnection(chan, host);
        while (con.state == ConnectionState.Connecting)
        {
            con.PumpEvents();
            await Task.Delay(0);
        }
        return con;
    }

    public byte[] CalculateChallenge(byte[] challenge, ulong id)
    {
        byte[] data = new byte[challenge.Length+8];
        BitConverter.GetBytes(id).CopyTo(data, challenge.Length);
        challenge.CopyTo(data, 0);
        return sha.ComputeHash(data);
    }

    public byte[] CalculateCurrentChallenge(ulong id)
    {
        return CalculateChallenge(currentChallenge, id);
    }

    public void UpdateMyChallenge()
    {
        myChallenge = CalculateCurrentChallenge(steamid);
    }

    public bool SendMessageTo(SteamNetworkingIdentity ident, Message id, byte[] data)
    {
        byte[] msg = new byte[data.Length+myChallenge.Length+1];
        msg[0] = (byte)id;
        myChallenge.CopyTo(msg, 1);
        data.CopyTo(msg, myChallenge.Length+1);
        return Utils.SendSteamMessage(ref ident, channel, msg);
    }

    public void SendMessage(Message id, byte[] data)
    {
        //SteamNetworkingMessages.SendMessageToUser()
        foreach (var peer in peers)
        {
            //SendMessageTo(peer, id, data);
            peer.SendRaw(id, data);
        }
    }

    public void UpdateChallenge(byte[] challenge)
    {
        currentChallenge = challenge;
        foreach (var peer in peers)
        {
            peer.UpdateChallenge();
        }
        UpdateMyChallenge();
    }

    public void InformChallengeUpdate(byte[] newChallenge)
    {
        SendMessage(Message.ChallengeUpdate, newChallenge);
    }

    public void RefreshChallenge()
    {
        byte[] newChallenge = new byte[32];
        using(var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(newChallenge);
        }
        InformChallengeUpdate(newChallenge);
        UpdateChallenge(newChallenge);
    }

    /*void SendChatMessage(string msg)
    {
        SendMessage(Message.ChatMessage, Encoding.UTF8.GetBytes(msg));
    }*/

    /*void SendPartyJoin()
    {
        
    }*/

    void SendKeepalive()
    {
        SendMessage(Message.Keepalive, []);
    }

    void SendHostRequest()
    {
        byte[] data = Utils.ConvertFrom<HostRequest>(new HostRequest()
        {
            join_time = connectionTime
        });
        SendMessage(Message.HostRaffle, data);
    }

    void AssertBecomeHost()
    {
        SendMessage(Message.HostAssert, []);
    }

    void PumpEvents()
    {
        if (state < ConnectionState.Disconnected)
        {
            IntPtr[] ptr = new IntPtr[1];
            int count = SteamNetworkingMessages.ReceiveMessagesOnChannel(channel, ptr, 1);
            if (count > 0)
            {
                var msg = SteamNetworkingMessage_t.FromIntPtr(ptr[0]);
                if (msg.m_identityPeer.GetSteamID64() == host.GetSteamID64())
                {
                    byte[] message = new byte[msg.m_cbSize];
                    Marshal.Copy(msg.m_pData, message, 0, message.Length);
                    Message m = (Message)message[0];
                    byte[] msgdat = new byte[msg.m_cbSize-33];
                    Array.Copy(message, 33, msgdat, 0, msgdat.Length);
                    if (m == Message.Challenge)
                    {
                        byte[] response = CalculateChallenge(msgdat, steamid);
                        Utils.SendSteamMessage(ref host, channel, response);
                        state = ConnectionState.AwaitingChallenge;
                    } else if (m == Message.ChallengeUpdate)
                    {
                        
                    } else if (m == Message.PartyPlayers)
                    {
                        
                    }
                }
            }
        }
    }
}