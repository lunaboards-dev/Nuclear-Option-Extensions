using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using Steamworks;
using UnityEngine;

namespace NOX.Party;

class PartyManager : MonoBehaviour
{
    //HSteamListenSocket sock;

    void Awake()
    {
        //sock = SteamNetworkingSockets.CreateListenSocketP2P(0, 0, []);
    }
}