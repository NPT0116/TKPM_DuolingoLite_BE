using System;
using System.Threading.Tasks;
using Application.Common.Interface;
using Application.Common.Utls;
using Domain.CacheKeyFactory;
using Domain.Entities.Ranking;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Services;

public class RankingService : IRankingService
{
    private readonly ApplicationDbContext _context;
    private readonly IDistributedCache _cache;

     public RankingService(ApplicationDbContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
    }
    public async Task<Ranking> GetTopRanking(int Top )
    {
        var cacheKey = $"TopRanking_{Top}";
        var cachedRanking = _cache.GetString(cacheKey);

        if (cachedRanking != null)
        {
            Console.WriteLine("Cache hit for TopRanking");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Ranking>(cachedRanking);
        }

var ranking = await _context.UserStats
    .Join(_context.UserProfiles,
          stat => stat.UserId,
          profile => profile.UserId,
          (stat, profile) => new { Stat = stat, Profile = profile })
    .OrderByDescending(x => x.Stat.ExperiencePoint)
    .ThenBy(x => x.Profile.NickName)
    .Take(Top)
    .ToListAsync();

var listUserRanking = new List<UserRanking>();
for (int i = 0; i < ranking.Count; i++)
{
    var entry = ranking[i];
    var rank = i + 1;
    var userRanking = new UserRanking(entry.Stat.UserId, rank, entry.Profile.NickName, entry.Stat.ExperiencePoint);
    listUserRanking.Add(userRanking);
}

        var result = new Ranking(Top)
        {
            UserRankings = listUserRanking
        };
        // Convert to Ranking object

        var rankingJson = Newtonsoft.Json.JsonConvert.SerializeObject(result);
        _cache.SetString(cacheKey, rankingJson);

        return result;
    }

public async Task<UserRanking> GetUserRanking(Guid userId)
{
    var cacheKey = $"UserRanking_{userId}";
    var cachedRanking = _cache.GetString(cacheKey);
    if (cachedRanking != null)
    {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<UserRanking>(cachedRanking);
    }

    // Lấy thông tin của user cần tính thứ hạng
    var userStat = await _context.UserStats
        .FirstOrDefaultAsync(x => x.UserId == userId);

    if (userStat == null)
    {
        throw new Exception("User not found");
    }

    // Tính thứ hạng: số người có điểm XP cao hơn + 1
    int rank = await _context.UserStats.CountAsync(x => x.ExperiencePoint > userStat.ExperiencePoint) + 1;
    var UserProfile = await _context.UserProfiles
        .FirstOrDefaultAsync(x => x.UserId == userId);
    if (UserProfile == null)
    {
        throw new Exception("UserProfile not found");
    }
    var userRanking = new UserRanking(userStat.UserId, rank, UserProfile.NickName, userStat.ExperiencePoint);
    // Lưu vào cache với TTL là 1 giờ
    var options = new DistributedCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromHours(1));
    var userRankingJson = Newtonsoft.Json.JsonConvert.SerializeObject(userRanking);
    _cache.SetString(cacheKey, userRankingJson, options);
    // Trả về kết quả

    return userRanking;
}

    public Task UpdateTopRanking(int top)
    {
        var cacheKey = RankingCacheKey.TopRanking(top);
        var cachedRanking = _cache.GetString(cacheKey);

        if (cachedRanking != null)
        {
            var ranking = Newtonsoft.Json.JsonConvert.DeserializeObject<Ranking>(cachedRanking);
            
            _cache.Remove(cacheKey);
        }

        return Task.CompletedTask;
    }

    public Task UpdateUserRanking(Guid userId)
    {
        var cacheKey = RankingCacheKey.GetUserRanking(userId);
        var cachedRanking = _cache.GetString(cacheKey);

        if (cachedRanking != null)
        {
            var userRanking = Newtonsoft.Json.JsonConvert.DeserializeObject<UserRanking>(cachedRanking);
            _cache.Remove(cacheKey);
        }

        return Task.CompletedTask;
    }
}
