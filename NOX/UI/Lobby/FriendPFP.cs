using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using NuclearOption.MissionEditorScripts;

namespace NOX.UI.Lobby;

class FriendPFP : MonoBehaviour {
    RawImage image;
    Outline outline;
    public RectTransform tf;
    
    CSteamID user;

    public Color color
    {
        get => outline.effectColor;
        set => outline.effectColor = value;
    }

    void Awake()
    {
        image = gameObject.AddComponent<RawImage>();
        outline = gameObject.AddComponent<Outline>();
        image.color = Color.white;
        tf = gameObject.GetComponent<RectTransform>();
        tf.SetRectSize(new Vector2(80, 80));
        tf.pivot = new Vector2(0, 0);
        
        outline.effectColor = Color.grey;
        outline.effectDistance = new Vector2(2, -2);
    }

    internal void SetID(CSteamID id)
    {
        user = id;
    }

    public void UpdateTexture(Texture2D t)
    {
        image.texture = t;
        image.uvRect = new Rect(0, 1, 1, -1);
    }
}