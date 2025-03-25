using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedKernel;

namespace Domain.Entities.Learning.Questions.QuestionOptions
{
    public class QuestionOptionError
    {
        public static Error IsCorrectFieldMissing 
            => Error.Validation("IsCorrectFieldMissing", "Multiple choice question option must include the 'IsCorrect' field.");

        public static Error SourceTypeFieldMissing 
            => Error.Validation("SourceTypeFieldMissing", "Matching question option must include the 'SourceType' field.");

        public static Error TargetTypeFieldMissing
            => Error.Validation("TargetTypeFieldMissing", "Matching question option must include the 'TargetType' field.");

        public static Error TargetTypeMustBeDifferentFromSourceType
            => Error.Validation("TargetTypeMustBeDifferentFromSourceType", "Matching question option 'TargetType' must be different from 'SourceType'.");

        public static Error QuestionTypeNotSupported
            => Error.Validation("QuestionTypeNotSupported", "Question type is not supported.");
    }
}