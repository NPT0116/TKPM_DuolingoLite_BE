using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Questions.Enums;
using SharedKernel;

namespace Domain.Entities.Learning.Questions.QuestionOptions.Validator
{
    public interface IQuestionOptionValidator
    {
        Result Validate(QuestionType type, List<QuestionOptionBase> options);
    }
}