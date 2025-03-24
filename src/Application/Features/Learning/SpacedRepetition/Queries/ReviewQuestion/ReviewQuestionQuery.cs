using System;
using Application.Abstractions.Messaging;
using Application.Features.Learning.Question.Queries.GetAQuestionFromLessionId;
using MediatR;
using SharedKernel;

namespace Application.Features.Learning.SpacedRepetition.Queries.ReviewQuestion;

public record ReviewQuestionQueryParam(
    Guid RecordId
) ;

public record ReviewQuestionQuery (ReviewQuestionQueryParam QueryParam) : IQuery<QuestionDto>;
