namespace BlockedCountries.API.Controllers;

[Route("api/logs/")]
[ApiController]
public class LogController : ControllerBase
{
    [HttpGet("blocked-attempts")]
    public IActionResult GetBlockedLogs(int page = 1, int pageSize = 10)
    {
        var logs = BlockedAttempetLogger.GetLogs(page, pageSize);
        return Ok(logs);
    }
}