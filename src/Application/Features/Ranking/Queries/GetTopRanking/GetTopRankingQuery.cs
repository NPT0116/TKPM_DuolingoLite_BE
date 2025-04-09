using System;
using Application.Abstractions.Messaging;

namespace Application.Features.Ranking.Queries.GetTopRanking;

public record GetTopRankingQuery(int Top): IQuery<Domain.Entities.Ranking.Ranking>;

