using Application.Common.Interface;
using Domain.CacheKeyFactory;
using Domain.Entities.Ranking;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

public class UpdateRankingEventHandler : INotificationHandler<UpdateRankingEvent>
{
    private readonly IDistributedCache _cache;
    private readonly IRankingService _rankingService;
    
    public UpdateRankingEventHandler(IDistributedCache cache, IRankingService rankingService)
    {
        _cache = cache;
        _rankingService = rankingService;
    }

    public async Task Handle(UpdateRankingEvent notification, CancellationToken cancellationToken)
    {
        // Lấy cache cho top10 ranking
        var cacheKey = RankingCacheKey.TopRanking(10);
        var cachedRanking = await _cache.GetStringAsync(cacheKey, cancellationToken);
        // Console.WriteLine("UpdateRankingEventHandler: " + cacheKey);
        // Update riêng cache cho user (cập nhật ranking cá nhân)
        await _rankingService.UpdateUserRanking(notification.UserId);

        // Nếu có cache top10 tồn tại, kiểm tra xem có cần invalid hay không
        if (cachedRanking != null)
        {
            var ranking = Newtonsoft.Json.JsonConvert.DeserializeObject<Ranking>(cachedRanking);
            if (ranking != null && ranking.UserRankings.Any())
            {
                // Kiểm tra xem user đã nằm trong top10 chưa
                bool isUserInTop = ranking.UserRankings.Any(r => r.UserId == notification.UserId);

                // Giả sử notification có trường NewExperiencePoint chứa XP mới của user
                int newXp = notification.ExperiencePoint;

                // Lấy mốc XP thấp nhất trong top10
                int minXpInTop = ranking.UserRankings.Last().ExperiencePoint;

                // Nếu user nằm trong top10 hoặc XP mới vượt qua mốc thấp nhất trong top10
                if (isUserInTop || newXp > minXpInTop)
                {
                    // Invalidate cache top10 để lần sau lấy được dữ liệu mới nhất
                    // Console.WriteLine("Reset cache for TopRanking");
                  await  _rankingService.UpdateTopRanking(10);
                }
            }
            else
            {
                // Nếu không có cache top10, thì cập nhật lại cache
                // Console.WriteLine("Reset cache for TopRanking");
                await _rankingService.UpdateTopRanking(10);
            }
        }
    }
}
