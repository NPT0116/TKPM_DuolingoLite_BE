using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities.Learning.SpacedRepetition;
using Domain.Repositories;
using MediatR;
using SharedKernel;

namespace Application.Features.Learning.SpacedRepetition.Commands.UpdateReview
{
    public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, Result<bool>>
    {
        private readonly ISpacedRepetitionRepository _spacedRepetitionRepository;

        public UpdateReviewCommandHandler(ISpacedRepetitionRepository spacedRepetitionRepository)
        {
            _spacedRepetitionRepository = spacedRepetitionRepository;
        }

        public async Task<Result<bool>> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var record = await _spacedRepetitionRepository.GetByIdAsync(request.RecordId);
                if (record == null)
                {
                    return Result.Failure<bool>(SpacedRepetitionError.RecordNotFound);
                }

                record.UpdateBasedOnResult(request.IsCorrect);
                await _spacedRepetitionRepository.UpdateAsync(record);

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>(Error.Problem(
                    "SpacedRepetition.UpdateFailed",
                    $"Failed to update review record: {ex.Message}"));
            }
        }
    }
} 