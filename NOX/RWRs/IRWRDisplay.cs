namespace NOX.RWRs;

public interface IRWRDisplay
{
    public bool SupportFilters { get; }
    public string SystemName { get; }
    public void Contact(RWRContact contact);
    public void Init();
    public void Update();
    public void Destroy();
}