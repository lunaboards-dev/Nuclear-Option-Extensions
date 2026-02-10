using System;
using System.Collections.Generic;
using NuclearOption.MissionEditorScripts;
using NuclearOption.Networking.Lobbies;
using Steamworks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NOX.UI.Lobby;

class Friend : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    GameObject PFPHolder;
    GameObject NameHolder;
    GameObject StatusHolder;
    RectLabel Name;
    RectLabel Status;
    FriendPFP PFP;
    Image background;
    EventTrigger Trg;
    public RectTransform Tf;
    ulong id;

    void RefreshInfo(CSteamID cid)
    {
        Name.text.text = SteamFriends.GetFriendPersonaName(cid);
        if (SteamFriends.GetFriendGamePlayed(cid, out FriendGameInfo_t game))
        {
            if (game.m_gameID.AppID() == SteamUtils.GetAppID())
            {
                PFP.color = Color.green;
                Status.text.text = "In-game";
                if (game.m_unGameIP > 0)
                {
                    Status.text.text = "Playing on a server";
                }
            } else
            {
                PFP.color = Color.cyan;
                Status.text.text = "Playing other game.";
            }
        } else if ((SteamFriends.GetFriendPersonaState(cid) & EPersonaState.k_EPersonaStateOnline) > 0) {
            PFP.color = Color.cyan;
            Status.text.text = "Online";
        } else
        {
            PFP.color = Color.grey;
            Status.text.text = "Offline";
        }
    }

    void Awake()
    {
        Tf = gameObject.AddComponent<RectTransform>();
        Tf.SetRectSize(new Vector2(500, 80));
        Tf.pivot = new Vector2(0, 0);

        background = gameObject.AddComponent<Image>();
        background.color = Color.black;

        PFPHolder = new GameObject("NOXFriendPFP");
        PFP = PFPHolder.AddComponent<FriendPFP>();
        print($"PFP: {PFP}, PFP.tf: {PFP.tf}, Tf: {Tf}");
        PFP.tf.SetParent(Tf, false);
        PFP.tf.localPosition = new Vector3(5, 0, 0);

        NameHolder = new GameObject("NOXFriendName");
        Name = NameHolder.AddComponent<RectLabel>();
        Name.rect.SetParent(Tf, false);
        Name.rect.pivot = new Vector2(0, 0);
        Name.rect.SetRectSize(new Vector2(500-80, 40));
        Name.text.alignment = TextAnchor.LowerLeft;
        Name.transform.localPosition = new Vector3(90, 40, 0);
        Name.text.font = Resources.UIFont;
        Name.text.fontSize = 30;
        Name.text.text = "<NOT LOADED>";

        StatusHolder = new GameObject("NOXStatusHolder");
        Status = StatusHolder.AddComponent<RectLabel>();
        Status.rect.SetParent(Tf, false);
        Status.rect.pivot = new Vector2(0, 1);
        Status.rect.SetRectSize(new Vector2(500-80, 40));
        Status.text.alignment = TextAnchor.UpperLeft;
        Status.rect.localPosition = new Vector3(90, 40, 0);
        Status.text.font = Resources.UIFont;
        Status.text.fontSize = 30;
        Status.text.text = "Offline";

        Trg = gameObject.AddComponent<EventTrigger>();

        //Steam.Instance.OnFriendChange += UserRefresh;
        Steam.Instance.OnFriendUpdate += FriendRefresh;
    }

    void Update()
    {
        if (id > 0)
            FriendRefresh(Steam.Instance.Cached[id]);
    }

    public void SetID(CSteamID id)
    {
        this.id = id.m_SteamID;
        var friend = Steam.Instance.Cached[id.m_SteamID];
        FriendRefresh(friend);
        //RefreshInfo(id);
        //PFP.SetID(id);
    }

    void OnDestroy()
    {
        //Steam.Instance.OnFriendChange -= UserRefresh;
        Steam.Instance.OnFriendUpdate -= FriendRefresh;
    }

    Dictionary<FriendStatus, (string info, Color color)> Map = new(){
        {FriendStatus.Playing, ("Playing on a server", Color.green)},
        {FriendStatus.InGame, ("In-game", Color.green)},
        {FriendStatus.Online, ("Online", new Color(0f, 0.5f, 0.7f))},
        {FriendStatus.Offline, ("Offline", Color.grey)}
    };

    public void ReqFailed() {}

    public void ServerResponded(gameserveritem_t server)
    {
        Status.text.text = "Playing on "+server.GetServerName();
    }

    public void FriendRefresh(Steam.SteamFriend friend)
    {
        (string info, Color color) stat = Map[friend.status];
        PFP.color = stat.color;
        if (friend.status == FriendStatus.Online && friend.in_game)
            Status.text.text = "In other game";
        else if (friend.status == FriendStatus.InGame)
            SteamMatchmakingServers.PingServer(friend.ip, friend.port, new ISteamMatchmakingPingResponse(ServerResponded, ReqFailed));
        else
            Status.text.text = stat.info;
        Name.text.text = friend.name;
        PFP.UpdateTexture(friend.texture);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        background.color = new Color(1,1,1,0.1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        background.color = Color.black;
    }
}