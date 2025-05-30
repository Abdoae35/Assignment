namespace BlockedCountires.BLL.In_MemoryDic;

public class BlockedAttempt
{
    public string? IpAddress { get; set; } 
    public DateTime Timestamp { get; set; }
    public string? CountryCode { get; set; } 
    public bool Blocked { get; set; }
    public string? UserAgent { get; set; } 
}