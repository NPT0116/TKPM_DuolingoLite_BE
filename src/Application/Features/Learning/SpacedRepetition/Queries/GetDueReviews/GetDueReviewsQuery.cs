using System;
using MediatR;
using SharedKernel;

namespace Application.Features.Learning.SpacedRepetition.Queries.GetDueReviews
{
    public record GetDueReviewsQuery(
        Guid UserId,
        int Limit = 10,
        DateTime? Cursor = null
    ) : IRequest<Result<GetDueReviewsResponse>>;
} 