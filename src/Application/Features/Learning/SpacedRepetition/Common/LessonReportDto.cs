using System;
using System.Collections.Generic;

namespace Application.Features.Learning.SpacedRepetition.Common
{
    public class LessonReportDto
    {
        public Guid UserId { get; set; }
        public Guid LessonId { get; set; }
        public List<QuestionResultDto> Results { get; set; } = new();
    }

    public class QuestionResultDto
    {
        public Guid QuestionId { get; set; }
        public bool IsCorrect { get; set; }
    }
} 