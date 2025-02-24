using System;
using SharedKernel;

namespace Domain.Entities.Learning.Questions.QuestionOptions;

public static class QuestionWordsError
{
    public static Error QuestionWordNotFound => Error.NotFound("QuestionWordNotFound", "Question word not found.");
}
