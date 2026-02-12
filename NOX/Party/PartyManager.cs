using Steamworks;

namespace NOX.Party;

class PartyManager
{
    HSteamListenSocket sock;
    void Awake()
    {
        sock = SteamNetworkingSockets.CreateListenSocketP2P(0, 0, []);
        
    }
}