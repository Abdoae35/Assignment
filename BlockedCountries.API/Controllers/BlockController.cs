namespace BlockedCountries.API.Controllers;
/// <summary>
/// Manages country-level blocking and temporary blocking.
/// </summary>
[Route("api/countries/")]
[ApiController]
public class BlockController : ControllerBase
{
    private readonly IBlockedCountriesRepository _repository;

    public BlockController(IBlockedCountriesRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Gets a paginated list of blocked countries.
    /// </summary>
    /// <param name="page">Page number (default is 1).</param>
    /// <param name="pageSize">Page size (default is 10).</param>
    [HttpGet("blocked")]
    public async Task<IActionResult> Blocked(int page = 1, int pageSize = 10)
    {
        var  countries = await _repository.GetBlockedCountries(page, pageSize);
        return  Ok(countries);
    }
    
    
   
    /// <summary>
    /// Adds a country to the blocked list.
    /// </summary>
    /// <param name="countryCode">Country code to block (e.g., "US").</param>
    [HttpPost("block")]
    public async Task<IActionResult> Block(string countryCode)
    {
        var existing = await _repository.GetBlockedCountries();
        if (existing.Contains(countryCode))
            return Conflict("Country already blocked");

        await _repository.AddBlockedCountries(countryCode);
        return Ok();
    }
    
    /// <summary>
    /// Removes a country from the blocked list.
    /// </summary>
    /// <param name="countryCode">Country code to unblock.</param>
    [HttpDelete("block/{countryCode}")]
    public async Task<IActionResult> RemoveBlock(string countryCode)
    {
        var existing = await _repository.GetBlockedCountries();
        if (existing.Contains(countryCode))
        {
            await _repository.DeleteBlockedCountries(countryCode);
            return Ok("Country deleted");
            
        }

        return NotFound();
    }
    /// <summary>
    /// Temporarily blocks a country for a given duration.
    /// </summary>
    /// <param name="countryCode">Country code (2-letter ISO format).</param>
    /// <param name="durationMinutes">Duration in minutes (1-1440).</param>
    [HttpPost("temporal-block")]
    public async Task<IActionResult> TempBlock(string countryCode,int durationMinutes)
    {
        if (durationMinutes < 1 || durationMinutes > 1440)
            return BadRequest("Duration must be between 1 and 1440 minutes.");

        if (string.IsNullOrWhiteSpace(countryCode) || countryCode.Length != 2)
            return BadRequest("Invalid country code.");

       
        if (await _repository.IsTemporarilyBlocked(countryCode))
            return Conflict("Country already temporarily blocked.");

        var expiryTime = DateTime.UtcNow.AddMinutes(durationMinutes);
        await _repository.AddTemporaryBlock(countryCode.ToUpper(), expiryTime);

        return Ok("Temporary block added.");
    }
}