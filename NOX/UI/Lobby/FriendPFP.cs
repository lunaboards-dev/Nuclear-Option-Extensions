using UnityEngine;
using UnityEngine.UI;

namespace NOX.UI.Lobby;

class FriendPFP : MonoBehaviour {
    RawImage image;
    Texture2D tex;
    
    void Awake()
    {
        image = gameObject.AddComponent<RawImage>();

    }
}