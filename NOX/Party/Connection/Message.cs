namespace NOX.Party.Connection;
public enum Message : byte
{
    Invalid = 0,
    Error,
    Challenge,
    ChallengeUpdate,
    Keepalive,
    JoinRequest,
    JoinAccept,
    Kick,
    Leave,
    ChatMessage,
    HostRaffle,
    HostAssert,
    HostPromote,
    ServerJoin,
    TeamJoin,
    PartyPlayers,
    PartyUpdate,
    PartyInfo,
    Disband
}