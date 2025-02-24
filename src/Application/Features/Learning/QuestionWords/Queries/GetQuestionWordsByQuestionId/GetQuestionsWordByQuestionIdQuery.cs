using System;
using Application.Abstractions.Messaging;

namespace Application.Features.Learning.QuestionWords.Queries.GetQuestionWordsByQuestionId;


public record QuestionWordsDto(Guid Id, int Order, string Text, string AudioUrl);
public record GetQuestionsWordByQuestionIdQuery(Guid questionId): IQuery<QuestionWordsDto[]>;

