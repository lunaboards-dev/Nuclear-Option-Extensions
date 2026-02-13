using System;
using System.Collections.Generic;
using System.Linq;
using NOX.Hooks;
using Steamworks;
using UnityEngine;

namespace NOX;

enum FriendStatus
{
    Playing, InGame, Online, Offline
}

class Steam : MonoBehaviour
{

    public class SteamFriend
    {
        public Texture2D texture;
        public string name;
        public uint ip;
        public ushort port;
        public ushort qport;
        public AppId_t game_id;
        public CSteamID csid;
        public CSteamID lobby;
        public ulong id;
        public bool dirty;
        public bool dirty_pfp;
        public bool online;
        public bool in_game;
        public FriendStatus status;
        public EPersonaState state;
    }

    private CallResult<PersonaStateChange_t> friendListChange;
    CallResult<AvatarImageLoaded_t> imageLoaded;
    static readonly EFriendFlags friend_flags = EFriendFlags.k_EFriendFlagImmediate;
    public List<CSteamID> Friends = [];
    public List<ulong> FriendIDs = [];
    public Dictionary<ulong, SteamFriend> Cached = [];

    static GameObject Holder;

    public static Steam Instance;
    public Action<PersonaStateChange_t> OnFriendChange;
    public Action<SteamFriend> OnFriendUpdate;

    internal static void Init()
    {
        Holder = new GameObject("NOXSteam");
        Instance = Holder.AddComponent<Steam>();
    }

    void _OnFriendChange(PersonaStateChange_t c)
    {
        print("OnFriendChange");
    }

    void _OnFriendUpdate(SteamFriend c)
    {
        //print("OnFriendUpdate");
    }

    void OnEnable()
    {
        friendListChange = CallResult<PersonaStateChange_t>.Create(FriendsListUpdate);
        imageLoaded = CallResult<AvatarImageLoaded_t>.Create(LoadImage);
        OnFriendChange = _OnFriendChange;
        OnFriendUpdate = _OnFriendUpdate;
    }

    void FriendsListUpdate(PersonaStateChange_t person, bool failure)
    {
        if (failure) return;
        UserRefresh(person);
        RefreshFriends();
    }

    internal void RefreshFriends(bool force = false)
    {
        Friends.Clear();
        FriendIDs.Clear();
        int count = SteamFriends.GetFriendCount(friend_flags);
        Plugin.Logger.LogDebug($"Steam reports {count} friends.");
        for (int i = 0; i < count; i++)
        {
            CSteamID friend = SteamFriends.GetFriendByIndex(i, friend_flags);
            if (friend.IsValid())
            {
                //Logger.LogDebug($"FRIEND {i}: {friend.m_SteamID}");
                Friends.Add(friend);
                FriendIDs.Add(friend.m_SteamID);
                RefreshFriend(friend, force);
            }
        }
        SortFriends();
        Plugin.Logger.LogDebug($"Registered {Friends.Count} friends.");
    }

    void RefreshBasicInfo(CSteamID id, SteamFriend friend)
    {
        //print($"RefreshBasicInfo {id.m_SteamID} {friend}");
        friend.name = SteamFriends.GetFriendPersonaName(id);
        friend.state = SteamFriends.GetFriendPersonaState(id);
        friend.online = (friend.state & EPersonaState.k_EPersonaStateOnline) > 0;
        friend.in_game = false;
        if (SteamFriends.GetFriendGamePlayed(id, out FriendGameInfo_t game))
        {
            friend.in_game = true;
            friend.game_id = game.m_gameID.AppID();
            friend.ip = game.m_unGameIP;
            friend.port = game.m_usGamePort;
            friend.qport = game.m_usQueryPort;
            friend.lobby = game.m_steamIDLobby;
            if (friend.game_id == SteamUtils.GetAppID())
            {
                if (friend.ip > 0)
                {
                    friend.status = FriendStatus.Playing;
                } else
                {
                    friend.status = FriendStatus.InGame;
                }
            } else
            {
                friend.status = FriendStatus.Online;
            }
        } else
        {
            friend.status = friend.online ? FriendStatus.Online : FriendStatus.Offline;
        }
        friend.dirty = false;
    }

    void RefreshFriend(CSteamID id, bool force = false)
    {
        ulong lid = id.m_SteamID;
        bool should_update = false;
        if (Cached.TryGetValue(lid, out SteamFriend friend))
        {
            if (friend.dirty || force)
            {
                should_update = true;
                RefreshBasicInfo(id, friend);
            }
            if (friend.dirty_pfp || force)
            {
                should_update = true;
                SteamFriends.GetLargeFriendAvatar(id);
            }
            if (should_update) {
                if (OnFriendUpdate != null)
                    OnFriendUpdate(friend);
            }
        } else
        {
            should_update = true;
            SteamFriend f = new()
            {
                id = id.m_SteamID,
                csid = id
            };
            Cached.Add(lid, f);
            RefreshBasicInfo(id, f);
            int hand = SteamFriends.GetLargeFriendAvatar(id);
            RealLoadImage(hand, f);
            if (should_update) {
                //print("Friend: "+f);
                OnFriendUpdate?.Invoke(f);
            }
        }
    }

    void SortFriends()
    {
        Friends.Sort(delegate(CSteamID left, CSteamID right)
        {
            var lf = Cached[left.m_SteamID];
            var rf = Cached[right.m_SteamID];
            if (lf.status != rf.status)
                return lf.status.CompareTo(rf.status);
            if (lf.name == null || rf.name == null) return 0;
            return lf.name.CompareTo(rf.name);
        });
    }

    void RealLoadImage(int hand, SteamFriend friend)
    {
        if (!SteamUtils.GetImageSize(hand, out uint tw, out uint th))
        {
            Plugin.Logger.LogError($"Failed to load avatar for user {friend.id}");
        }
        friend.texture = new Texture2D((int)tw, (int)th, TextureFormat.RGBA32, false, true);
        byte[] tmp = new byte[tw*th*4];
        if (!SteamUtils.GetImageRGBA(hand, tmp, tmp.Length))
        {
            Plugin.Logger.LogError($"Failed to load avatar for user {friend.id}");
        }
        friend.texture.LoadRawTextureData(tmp);
        friend.texture.Apply();
        friend.dirty_pfp = false;

        if (OnFriendUpdate != null)
            OnFriendUpdate(friend);
    }

    void LoadImage(AvatarImageLoaded_t avatar, bool failure)
    {
        if (failure || !Cached.TryGetValue(avatar.m_steamID.m_SteamID, out SteamFriend friend)) return;
        int w = avatar.m_iWide;
        int h = avatar.m_iTall;
        RealLoadImage(avatar.m_iImage, friend);
    }

    internal bool IsFriend(ulong id)
    {
        return FriendIDs.Contains(id);
    }

    internal CSteamID ToCSID(ulong id)
    {
        return Friends.Where(ID => ID.m_SteamID == id).First();
    }

    static readonly EPersonaChange should_refresh = EPersonaChange.k_EPersonaChangeName | EPersonaChange.k_EPersonaChangeRichPresence | EPersonaChange.k_EPersonaChangeStatus | EPersonaChange.k_EPersonaChangeComeOnline | EPersonaChange.k_EPersonaChangeGoneOffline;

    void UserRefresh(PersonaStateChange_t user)
    {
        //CSteamID cid = ToCSID(user.m_ulSteamID);
        if (!Cached.TryGetValue(user.m_ulSteamID, out SteamFriend sf)) return;
        if ((user.m_nChangeFlags & should_refresh) > 0)
        {
            sf.dirty = true;
        }

        if ((user.m_nChangeFlags & EPersonaChange.k_EPersonaChangeAvatar) > 0)
            sf.dirty_pfp = true;
    }

    // idk why NO doesn't seem to do this itself
    internal class SteamCallbackRunner : MonoBehaviour
    {
        void Update()
        {
            SteamAPI.RunCallbacks();
        }
    }
}