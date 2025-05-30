

namespace BlockedCountires.BLL.In_MemoryDic;

public class CountriesDictionary
{

    // Thread-safe storage for unique country codes
    public static ConcurrentDictionary<string, byte> CountryDic { get; set; } = new();
    
    
}