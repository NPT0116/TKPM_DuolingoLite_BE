using SharedKernel;

namespace Domain.Entities.Learning.SpacedRepetition
{
    public static class SpacedRepetitionError
    {
        public static Error UserIdRequired() => Error.Validation(
            "SpacedRepetition.UserIdRequired", 
            "User ID is required for creating a spaced repetition record."
        );

        public static Error QuestionIdRequired() => Error.Validation(
            "SpacedRepetition.QuestionIdRequired", 
            "Question ID is required for creating a spaced repetition record."
        );
        
        public static Error RecordNotFound => Error.NotFound(
            "SpacedRepetition.RecordNotFound",
            "Spaced repetition record not found."
        );
    }
} 