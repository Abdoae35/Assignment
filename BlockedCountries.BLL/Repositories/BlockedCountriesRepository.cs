namespace BlockedCountires.BLL.Repositories;

public class BlockedCountriesRepository : IBlockedCountriesRepository
{
    private readonly CountriesDictionary _blockedCountries;
    private readonly Dictionary<string, DateTime> _tempBlocked = new();

    public BlockedCountriesRepository()
    {
        _blockedCountries = new CountriesDictionary();
    }

    public Task<List<string>> GetBlockedCountries(int page = 1, int pageSize = 6)
    {
        var skip = (pageSize * page) - pageSize;
        var pagedCountries = CountriesDictionary.CountryDic
            .Keys
            .Skip(skip)
            .Take(pageSize)
            .ToList();

        return Task.FromResult(pagedCountries);
    }

    public Task<string?> AddBlockedCountries(string addCountry)
    {
        var added = CountriesDictionary.CountryDic.TryAdd(addCountry, 0); 
        return Task.FromResult<string?>(added ? "Successfull" : "Country code already exists");
    }

    public Task DeleteBlockedCountries(string deleteBlockedCountries)
    {
        CountriesDictionary.CountryDic.TryRemove(deleteBlockedCountries, out _);
        return Task.CompletedTask;
    }

    public Task AddTemporaryBlock(string countryCode, DateTime expiryTime)
    {
        _tempBlocked[countryCode] = expiryTime;
        return Task.CompletedTask;
    }

    public Task<bool> IsTemporarilyBlocked(string countryCode)
    {
        return Task.FromResult(_tempBlocked.TryGetValue(countryCode, out var expiry) && expiry > DateTime.UtcNow);
    }

    public Task<List<(string CountryCode, DateTime ExpiryTime)>> GetAllTemporaryBlocks()
    {
        var list = _tempBlocked
            .Select(kv => (CountryCode: kv.Key, ExpiryTime: kv.Value))
            .ToList();

        return Task.FromResult(list);
    }


    public Task RemoveTemporaryBlock(string countryCode)
    {
        _tempBlocked.Remove(countryCode);
        return Task.CompletedTask;
    }
}