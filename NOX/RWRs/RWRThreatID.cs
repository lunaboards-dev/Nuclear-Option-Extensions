#nullable enable
namespace NOX.RWRs;

public enum RWRThreatType
{
    AirIntercept = 1,
    Attacker = 2,
    AEW = 4,
    SAM = 8,
    AAA = 16,
    Ship = 32,
    FCS = 64,
    EarlyWarning = 128,
    Missile = 256,
    Unknown = 0x8000
}
public struct RWRThreatID
{
    public string Name;
    public string? Display;
    public string Band;
    public RWRThreatType Class;
}