using System;
using System.Security.Principal;
using Application.Abstractions.Messaging;
using Application.Common.Interface;
using Application.Interface;
using Domain.Entities.Ranking;
using Domain.Entities.Users;
using SharedKernel;

namespace Application.Features.Ranking.Queries.GetUserRanking;

public class GetUserRankingQueryHandler : IQueryHandler<GetUserRankingQuery, UserRanking>
{
    private readonly IRankingService _rankingService;
    private readonly IIdentityService _identityService;
    public GetUserRankingQueryHandler(IRankingService rankingService, IIdentityService identityService)
    {
        _rankingService = rankingService;
        _identityService = identityService;
    }
    public async Task<Result<UserRanking>> Handle(GetUserRankingQuery request, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetCurrentUserAsync();
        if (user == null)
        {
            Console.WriteLine("User not found");
            return Result.Failure<UserRanking>(UserError.Unauthorized());
        }
        var ranking = await _rankingService.GetUserRanking(user.Id);

        return ranking;
    }
}
