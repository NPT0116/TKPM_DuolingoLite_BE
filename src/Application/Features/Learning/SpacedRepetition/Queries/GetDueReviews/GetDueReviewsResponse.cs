using System;
using System.Collections.Generic;
using Application.Features.Learning.SpacedRepetition.Common;

namespace Application.Features.Learning.SpacedRepetition.Queries.GetDueReviews
{
    public class GetDueReviewsResponse
    {
        public List<SpacedRepetitionDto> DueReviews { get; init; } = new();
        public DateTime? NextCursor { get; init; }
        public bool HasMore { get; init; }
    }
} 