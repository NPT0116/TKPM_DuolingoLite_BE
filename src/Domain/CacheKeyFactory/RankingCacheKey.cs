using System;

namespace Domain.CacheKeyFactory;

public static class RankingCacheKey
{
    public static string TopRanking(int Top)
    {
        var cacheKey = $"TopRanking_{Top}";
        return cacheKey;
    }

    public static string GetUserRanking(Guid UserId)
    {
        var cacheKey = $"UserRanking_{UserId}";
        return cacheKey;
    }
}
