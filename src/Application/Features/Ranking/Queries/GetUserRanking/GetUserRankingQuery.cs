using System;
using Application.Abstractions.Messaging;
using Domain.Entities.Ranking;

namespace Application.Features.Ranking.Queries.GetUserRanking;



public class GetUserRankingQuery(): IQuery<UserRanking>;