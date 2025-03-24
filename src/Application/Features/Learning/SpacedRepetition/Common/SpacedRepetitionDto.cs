using System;

namespace Application.Features.Learning.SpacedRepetition.Common
{
    public class SpacedRepetitionDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid QuestionId { get; set; }
        public DateTime LastReview { get; set; }
        public DateTime NextReview { get; set; }
        public int RepetitionCount { get; set; }
        public double EasinessFactor { get; set; }
    }
} 