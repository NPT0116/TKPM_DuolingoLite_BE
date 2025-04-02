using System;
using Domain.Entities.Ranking;

namespace Application.Common.Interface;

public interface IRankingService
{
    public Task<Ranking> GetTopRanking(int Top = 10);
    public Task<UserRanking> GetUserRanking(Guid userId);
    public Task  UpdateUserRanking(Guid userId);
    public Task UpdateTopRanking(int top);


}
