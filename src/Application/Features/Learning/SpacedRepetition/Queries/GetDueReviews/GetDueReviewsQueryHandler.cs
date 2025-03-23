using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Utls;
using Application.Features.Learning.SpacedRepetition.Common;
using Domain.Repositories;
using MediatR;
using SharedKernel;

namespace Application.Features.Learning.SpacedRepetition.Queries.GetDueReviews
{
    public class GetDueReviewsQueryHandler : IRequestHandler<GetDueReviewsQuery, Result<GetDueReviewsResponse>>
    {
        private readonly ISpacedRepetitionRepository _spacedRepetitionRepository;

        public GetDueReviewsQueryHandler(ISpacedRepetitionRepository spacedRepetitionRepository)
        {
            _spacedRepetitionRepository = spacedRepetitionRepository;
        }

        public async Task<Result<GetDueReviewsResponse>> Handle(GetDueReviewsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Get one more than requested to check if there are more results
                var limit = request.QueryParam.Limit + 1;
                
                var records = await _spacedRepetitionRepository.GetDueReviewsAsync(
                    request.QueryParam.UserId, 
                    limit, 
                    request.QueryParam.Cursor);
                PrintUtils.PrintAsJson(records);
                bool hasMore = records.Count > request.QueryParam.Limit;
                if (hasMore)
                {
                    // Remove the extra item we fetched
                    records.RemoveAt(records.Count - 1);
                }

                // Map to DTOs
                var dueReviews = records.Select(r => new SpacedRepetitionDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    QuestionId = r.QuestionId,
                    LastReview = r.LastReview,
                    NextReview = r.NextReview,
                    RepetitionCount = r.RepetitionCount,
                    EasinessFactor = r.EasinessFactor
                }).ToList();

                // Get the next cursor (NextReview of the last record)
                DateTime? nextCursor = null;
                if (records.Any())
                {
                    nextCursor = records.Last().NextReview;
                }

                return Result.Success(new GetDueReviewsResponse
                {
                    DueReviews = dueReviews,
                    NextCursor = nextCursor,
                    HasMore = hasMore
                });
            }
            catch (Exception ex)
            {
                return Result.Failure<GetDueReviewsResponse>(Error.Problem(
                    "SpacedRepetition.QueryFailed",
                    $"Failed to retrieve due reviews: {ex.Message}"));
            }
        }
    }
} 