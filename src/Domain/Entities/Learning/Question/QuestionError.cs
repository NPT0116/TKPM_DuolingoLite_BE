using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedKernel;

namespace Domain.Entities.Question
{
    public class QuestionError
    {
        public static Error AllPromptsNull() => Error.Validation(
            "Question.AllPromptsNull",
            "At least one prompt must be provided"
        );
    }
}