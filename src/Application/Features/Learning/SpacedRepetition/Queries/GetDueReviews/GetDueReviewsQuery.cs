using System;
using MediatR;
using SharedKernel;

namespace Application.Features.Learning.SpacedRepetition.Queries.GetDueReviews
{
    public record GetDueReviewsQueryParam(  Guid UserId,
        int Limit = 10,
        DateTime? Cursor = null);
    public record GetDueReviewsQuery(
      GetDueReviewsQueryParam QueryParam
    ) : IRequest<Result<GetDueReviewsResponse>>;
} 