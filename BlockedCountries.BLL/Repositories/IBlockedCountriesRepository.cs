namespace BlockedCountires.BLL.Repositories;

public interface IBlockedCountriesRepository
{
     
     Task<List<string>> GetBlockedCountries(int page = 1, int pageSize = 6);
   

     Task<string?> AddBlockedCountries( string  addCountryCode);
    
     
     Task DeleteBlockedCountries( string RemoveBlock);
     
     
     Task AddTemporaryBlock(string countryCode, DateTime expiryTime);
     Task<bool> IsTemporarilyBlocked(string countryCode);
     Task<List<(string CountryCode, DateTime ExpiryTime)>> GetAllTemporaryBlocks();
     Task RemoveTemporaryBlock(string countryCode);
}