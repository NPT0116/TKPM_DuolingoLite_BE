using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedKernel;

namespace Domain.Entities.Learning.Questions
{
    public class QuestionError
    {
        public static Error AllPromptsNull() => Error.Validation(
            "Question.AllPromptsNull",
            "At least one prompt must be provided"
        );
        public static Error QuestionNotFound => Error.NotFound(
            "Question.QuestionNotFound",
            "Question not found."
        );
        public static Error InvalidQuestionConfiguration => Error.Validation(
            "Question.InvalidQuestionConfiguration",
            "Invalid question configuration."
        );
        public static Error InvalidOptionConfiguration => Error.Validation(
            "Question.InvalidOptionConfiguration",
            "Invalid option configuration."
        );

        public static Error EnglishTextRequired => Error.Validation(
            "Question.EnglishTextRequired",
            "English text is required in pronunication question."
        );

        public static Error EnglishOrVitenameseTextRequired => Error.Validation(
            "Question.EnglishOrVitenameseTextRequired",
            "English or Vietnamese text is required."
        );
    }
}