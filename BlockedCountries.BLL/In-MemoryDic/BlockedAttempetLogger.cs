namespace BlockedCountires.BLL.In_MemoryDic;

public static class BlockedAttempetLogger
{
    private static readonly List<BlockedAttempt> _logs = new();

    public static void Log(BlockedAttempt attempt)
    {
        _logs.Add(attempt);
    }

    public static IEnumerable<BlockedAttempt> GetLogs(int page = 1, int pageSize = 10)
    {
        return _logs
            .OrderByDescending(log => log.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
    }
}