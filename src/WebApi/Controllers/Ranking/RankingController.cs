using System;
using Application.Common.Interface;
using Application.Common.Utls;
using Application.Features.Ranking.Queries.GetUserRanking;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using WebApi.Extensions;
using WebApi.Infrastructure;

namespace WebApi.Controllers.Ranking;

   [Route("api/[controller]")]
    [ApiController]
public class RankingController: ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IRankingService _rankingService;
    public RankingController(IMediator mediator, IRankingService rankingService)
    {
        _rankingService = rankingService;
        _mediator = mediator;
    }
    [HttpGet("top")]
    public async Task<IActionResult> GetTopRanking()
    {
        var result = await _mediator.Send(new Application.Features.Ranking.Queries.GetTopRanking.GetTopRankingQuery(10));

        return result.Match(Ok, CustomResults.Problem);
    }
    [HttpGet("user-ranking")]
    public async Task<IActionResult> GetUserRanking()
    {
        var query = new GetUserRankingQuery();
        var result = await _mediator.Send(query);
        if (result.IsFailure)
        {
            Console.WriteLine("Request failed");
        }
        // PrintUtils.PrintAsJson(result);
        return result.Match(Ok, CustomResults.Problem);
    }
    [HttpPost("reset-ranking")]
    public async Task<IActionResult> UpdateRanking()
    {
        await _rankingService.UpdateTopRanking(10);
        var query = new GetUserRankingQuery();
        var result = await _mediator.Send(new Application.Features.Ranking.Queries.GetTopRanking.GetTopRankingQuery(10));

        return result.Match(Ok, CustomResults.Problem);
    }
}
