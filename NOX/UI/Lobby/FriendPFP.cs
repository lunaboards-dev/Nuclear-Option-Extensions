using UnityEngine;
using UnityEngine.UI;
using Steamworks;

namespace NOX.UI.Lobby;

class FriendPFP : MonoBehaviour {
    RawImage image;
    Texture2D tex;
    
    void Awake()
    {
        image = gameObject.AddComponent<RawImage>();
    }

    void SetID(CSteamID id)
    {
        SteamFriends.GetLargeFriendAvatar(id);
    }
}