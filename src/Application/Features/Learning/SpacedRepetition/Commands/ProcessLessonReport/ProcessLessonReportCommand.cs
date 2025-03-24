using System;
using System.Collections.Generic;
using Application.Features.Learning.SpacedRepetition.Common;
using MediatR;
using SharedKernel;

namespace Application.Features.Learning.SpacedRepetition.Commands.ProcessLessonReport
{
    public record ProcessLessonReportCommand(
        Guid UserId,
        Guid LessonId,
        List<QuestionResultDto> Results
    ) : IRequest<Result<bool>>;
} 