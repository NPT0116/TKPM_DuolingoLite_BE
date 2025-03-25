using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Questions.Enums;
using Microsoft.VisualBasic;
using SharedKernel;

namespace Domain.Entities.Learning.Questions.QuestionOptions.Validator
{
    public class QuestionOptionValidator : IQuestionOptionValidator
    {
        public Result Validate(QuestionType type, List<QuestionOptionBase> options)
        {
            return type switch
            {
                QuestionType.MultipleChoice => ValidateMultipleChoice(options),
                QuestionType.Matching => ValidateMatching(options),
                _ => Result.Success() // Or fail if unsupported
            };
        }

        private Result ValidateMultipleChoice(List<QuestionOptionBase> options)
        {
            if (!options.Any()) return Result.Failure(QuestionOptionError.NoOptions);
            var castOptions = options.Select(o => (MultipleChoiceQuestionOption) o).ToList();
            var correctCount = castOptions.Count(o => o.IsCorrect == true);

            if (correctCount == 0)
                return Result.Failure(QuestionOptionError.NoCorrectOption);

            if (correctCount > 1)
                return Result.Failure(QuestionOptionError.MultipleCorrectOptions);

            return Result.Success();
        }

        private Result ValidateMatching(List<QuestionOptionBase> options)
        {
            if (!options.Any())
                return Result.Failure(QuestionOptionError.NoOptions);

            // Maybe later: check matching logic for pairs, types, etc.
            return Result.Success();
        }
    }

}