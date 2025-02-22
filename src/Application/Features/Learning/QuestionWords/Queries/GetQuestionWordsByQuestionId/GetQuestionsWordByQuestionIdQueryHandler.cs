using System;
using Application.Abstractions.Messaging;
using Domain.Entities.Learning.Questions;
using Domain.Entities.Learning.Questions.QuestionOptions;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Learning.QuestionWords.Queries.GetQuestionWordsByQuestionId;

public class GetQuestionsWordByQuestionIdQueryHandler : IQueryHandler<GetQuestionsWordByQuestionIdQuery, QuestionWordsDto[]>
{
    private readonly IQuestionWordRepository _questionWordRepository;
    private readonly IQuestionRepository _questionRepository;
    public GetQuestionsWordByQuestionIdQueryHandler( IQuestionWordRepository questionWordRepository, IQuestionRepository questionRepository)
    {
        _questionWordRepository = questionWordRepository;
        _questionRepository = questionRepository;
    }
    public async Task<Result<QuestionWordsDto[]>> Handle(GetQuestionsWordByQuestionIdQuery request, CancellationToken cancellationToken)
    {
        var question = await _questionRepository.GetQuestionByIdAsync(request.questionId);
        if (question == null)
        {
            return Result.Failure<QuestionWordsDto[]>(QuestionError.QuestionNotFound);
        }
        var questionWord = await _questionWordRepository.GetQuestionWordsByQuestionIdAsync(request.questionId);
        if (questionWord == null)
        {
            return Result.Failure<QuestionWordsDto[]>(QuestionWordsError.QuestionWordNotFound);
        }

        var questionWordsDto = questionWord.Select(qw => new QuestionWordsDto(qw.Id, qw.Order, qw.Word.Content, qw.Word.Audio.Url)).ToArray();
        return questionWordsDto;
    }
}
