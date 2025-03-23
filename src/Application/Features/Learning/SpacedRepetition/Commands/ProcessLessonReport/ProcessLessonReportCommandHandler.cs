using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities.Learning.SpacedRepetition;
using Domain.Repositories;
using MediatR;
using SharedKernel;

namespace Application.Features.Learning.SpacedRepetition.Commands.ProcessLessonReport
{
    public class ProcessLessonReportCommandHandler : IRequestHandler<ProcessLessonReportCommand, Result<bool>>
    {
        private readonly ISpacedRepetitionRepository _spacedRepetitionRepository;
        private readonly IQuestionRepository _questionRepository;

        public ProcessLessonReportCommandHandler(
            ISpacedRepetitionRepository spacedRepetitionRepository,
            IQuestionRepository questionRepository)
        {
            _spacedRepetitionRepository = spacedRepetitionRepository;
            _questionRepository = questionRepository;
        }

        public async Task<Result<bool>> Handle(ProcessLessonReportCommand request, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var result in request.Results)
                {
                    // Verify the question exists
                    var question = await _questionRepository.GetQuestionByIdAsync(result.QuestionId);
                    if (question == null)
                    {
                        continue; // Skip invalid question IDs
                    }

                    // Check if record exists
                    var existingRecord = await _spacedRepetitionRepository.GetByUserAndQuestionAsync(
                        request.UserId, result.QuestionId);

                    if (existingRecord == null)
                    {
                        // Create new record
                        var recordResult = SpacedRepetitionRecord.Create(request.UserId, result.QuestionId);
                        if (recordResult.IsFailure)
                        {
                            continue; // Skip if creation fails
                        }

                        existingRecord = recordResult.Value;
                        // Apply the result immediately to the new record
                        existingRecord.UpdateBasedOnResult(result.IsCorrect);
                        await _spacedRepetitionRepository.AddAsync(existingRecord);
                    }
                    else
                    {
                        // Update existing record
                        existingRecord.UpdateBasedOnResult(result.IsCorrect);
                        await _spacedRepetitionRepository.UpdateAsync(existingRecord);
                    }
                }

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>(Error.Problem(
                    "SpacedRepetition.ProcessingFailed",
                    $"Failed to process lesson report: {ex.Message}"));
            }
        }
    }
} 