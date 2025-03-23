using System;
using MediatR;
using SharedKernel;

namespace Application.Features.Learning.SpacedRepetition.Commands.UpdateReview
{
    public record UpdateReviewCommand(
        Guid RecordId,
        bool IsCorrect
    ) : IRequest<Result<bool>>;
} 