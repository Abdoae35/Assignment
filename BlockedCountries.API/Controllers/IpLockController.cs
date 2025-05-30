namespace BlockedCountries.API.Controllers;
/// <summary>
/// Handles IP-related services such as lookup and block-checking.
/// </summary>
[ApiController]
[Route("api/ip")]
public class IpLookupController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string? _apiKey;
    private readonly HttpClient _httpClient;


    public IpLookupController(IHttpClientFactory httpClientFactory, IConfiguration configuration, HttpClient httpClient)
    {
        _httpClientFactory = httpClientFactory;
        _apiKey = configuration["ApiSetting:ApiKey"];
        _httpClient = httpClient;
        
    }

    /// <summary>
    /// Finds the country information for a given IP address.
    /// </summary>
    /// <param name="ipAddress">Optional IP address. If omitted, uses the caller's IP.</param>
    /// <returns>Country details like code, name, and ISP.</returns>
    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup(string? ipAddress)
    {
        
        if (string.IsNullOrEmpty(ipAddress))
        {
            
            ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            
            if (string.IsNullOrEmpty(ipAddress))
                return BadRequest("IP address is required.");
        }
        
        if (!IPAddress.TryParse(ipAddress, out var ip))
            return BadRequest("Invalid IP address format.");

        
        string url = $"https://api.ipgeolocation.io/v2/ipgeo?apiKey={_apiKey}&ip={ipAddress}";
        HttpResponseMessage response;

        try
        {
            response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException)
        {
            return StatusCode(503, "Failed to fetch data from IP API.");
        }
       
       
        var content = await response.Content.ReadAsStringAsync();
        
           
        using var jsonDoc = JsonDocument.Parse(content);
        
        
        var root = jsonDoc.RootElement;


        if (!root.TryGetProperty("location", out var locationProp))
            return StatusCode(500, "Missing 'location' data in API response.");

        string? countryCode2 = locationProp.TryGetProperty("country_code2", out var code2Prop) ? code2Prop.GetString() : null;
        string? countryName = locationProp.TryGetProperty("country_name", out var nameProp) ? nameProp.GetString() : null;


        string? city = locationProp.TryGetProperty("city", out var cityProp) ? cityProp.GetString() : null;

        var result = new
        {
            countryCode2,
            countryName,
            city
        };

        return Ok(result);
        
    }
    /// <summary>
    /// Verifies if the caller's IP belongs to a blocked country.
    /// </summary>
    /// <returns>Returns true if blocked; logs the attempt.</returns>

    [HttpGet("check-block")]
    public async Task<IActionResult> IsIpBlock()
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        
        if(string.IsNullOrEmpty(ip))
            return BadRequest();
       
        var url = $"https://api.ipgeolocation.io/v2/ipgeo?apiKey={_apiKey}&ip={ip}";

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
        }
        catch
        {
            return StatusCode(503, "Failed to fetch data from IP API.");
        }

        var content = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(content);
        var root = jsonDoc.RootElement;

        var countryCode = root
            .GetProperty("location")
            .GetProperty("country_code2")
            .GetString();

        var userAgent = Request.Headers["User-Agent"].ToString();

       
       var isBlocked = CountriesDictionary.CountryDic.ContainsKey(countryCode);

        
        BlockedAttempetLogger.Log(new BlockedAttempt
        {
            IpAddress = ip,
            CountryCode = countryCode,
            Blocked = isBlocked,
            Timestamp = DateTime.UtcNow,
            UserAgent = userAgent
        });

        return Ok(new { blocked = isBlocked, countryCode });

    }
    
    
    
    
}

