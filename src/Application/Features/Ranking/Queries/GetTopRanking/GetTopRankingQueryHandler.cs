using System;
using Application.Abstractions.Messaging;
using Application.Common.Interface;
using SharedKernel;

namespace Application.Features.Ranking.Queries.GetTopRanking;

public class GetTopRankingQueryHandler : IQueryHandler<GetTopRankingQuery, Domain.Entities.Ranking.Ranking>
{
    private readonly IRankingService _rankingService;
    public GetTopRankingQueryHandler(IRankingService rankingService)
    {
        _rankingService = rankingService;
    }
    public async Task<Result<Domain.Entities.Ranking.Ranking>> Handle(GetTopRankingQuery request, CancellationToken cancellationToken)
    {
        var ranking = await _rankingService.GetTopRanking(request.Top);
        if (ranking == null)
        {
            throw new Exception("Ranking not found");
        }
    return ranking;
    }
}
