using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Questions;
using SharedKernel;

namespace Domain.Entities.Learning.SpacedRepetition
{
    public class SpacedRepetitionRecord : Entity
    {
        public Guid UserId { get; private set; }
        public Guid QuestionId { get; private set; }
        public Question Question { get; set; }
        public DateTime LastReview { get; private set; }
        public DateTime NextReview { get; private set; }
        public int RepetitionCount { get; private set; }
        public double EasinessFactor { get; private set; }

        private SpacedRepetitionRecord() { }

        private SpacedRepetitionRecord(
            Guid userId,
            Guid questionId,
            DateTime lastReview,
            DateTime nextReview,
            int repetitionCount,
            double easinessFactor)
        {
            UserId = userId;
            QuestionId = questionId;
            LastReview = lastReview;
            NextReview = nextReview;
            RepetitionCount = repetitionCount;
            EasinessFactor = easinessFactor;
        }

        public static Result<SpacedRepetitionRecord> Create(
            Guid userId,
            Guid questionId)
        {
            if (userId == Guid.Empty)
            {
                return Result.Failure<SpacedRepetitionRecord>(SpacedRepetitionError.UserIdRequired());
            }

            if (questionId == Guid.Empty)
            {
                return Result.Failure<SpacedRepetitionRecord>(SpacedRepetitionError.QuestionIdRequired());
            }

            var now = DateTime.UtcNow;
            // Initial easiness factor according to SM-2 algorithm
            const double initialEasinessFactor = 2.5;
            // Start with repetition count 0
            const int initialRepetitionCount = 0;
            // Next review is set to now (making it immediately available for review)
            var nextReview = now;

            return Result.Success(new SpacedRepetitionRecord(
                userId,
                questionId,
                now,
                nextReview,
                initialRepetitionCount,
                initialEasinessFactor));
        }

        public void UpdateBasedOnResult(bool isCorrect)
        {
            var now = DateTime.UtcNow;
            LastReview = now;

            if (isCorrect)
            {
                // Increase repetition count
                RepetitionCount++;
                
                // Calculate new easiness factor according to SM-2 algorithm
                // We use a simplified quality rating: 5 for correct, 0 for incorrect
                const int qualityRating = 5; // Max quality for correct answer
                EasinessFactor = Math.Max(1.3, EasinessFactor + (0.1 - (5 - qualityRating) * (0.08 + (5 - qualityRating) * 0.02)));
                
                // Calculate next review interval based on SM-2
                int interval;
                if (RepetitionCount == 1)
                {
                    interval = 1; // 1 day after first successful review
                }
                else if (RepetitionCount == 2)
                {
                    interval = 6; // 6 days after second successful review
                }
                else
                {
                    // For repetition count > 2, use the formula: interval = previous_interval * EF
                    int previousInterval = (int)Math.Round((NextReview - LastReview).TotalDays);
                    interval = (int)Math.Round(previousInterval * EasinessFactor);
                }
                
                NextReview = now.AddDays(interval);
            }
            else
            {
                // Reset repetition count on incorrect answer
                RepetitionCount = 0;
                // Decrease easiness factor slightly but not below 1.3
                EasinessFactor = Math.Max(1.3, EasinessFactor - 0.2);
                // Set next review to tomorrow
                NextReview = now.AddDays(1);
            }
        }
    }
} 