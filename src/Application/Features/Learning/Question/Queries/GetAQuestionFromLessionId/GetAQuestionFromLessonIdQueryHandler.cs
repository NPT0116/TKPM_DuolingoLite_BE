
        using System;
using Application.Abstractions.Messaging;
using Application.Common.Utls; // nơi bạn để PrintUtils
using Domain.Entities.Learning.Lessons;
using Domain.Entities.Learning.Questions;
using Domain.Entities.Learning.Questions.Enums;
using Domain.Entities.Learning.Questions.QuestionOptions;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Learning.Question.Queries.GetAQuestionFromLessionId
{
    public class GetAQuestionFromLessonIdQueryHandler 
        : IQueryHandler<GetAQuestionFromLessonIdQuery, QuestionDto>
    {
        private readonly ILessonRepository _lessonRepository;

        public GetAQuestionFromLessonIdQueryHandler(ILessonRepository lessonRepository)
        {
            _lessonRepository = lessonRepository;
        }

        public async Task<Result<QuestionDto>> Handle(
            GetAQuestionFromLessonIdQuery request, 
            CancellationToken cancellationToken)
        {
            // 1. Lấy Lesson
            var lesson = await _lessonRepository.GetLessonByIdAsync(request.LessonId);
            if (lesson == null)
            {
                return Result.Failure<QuestionDto>(LessonError.LessonNotFound);
            }

            // 2. Lấy Question
            var question = lesson.Questions
                .FirstOrDefault(q => q.Order == request.QuestionOrder);
            if (question == null)
            {
                return Result.Failure<QuestionDto>(QuestionError.QuestionNotFound);
            }

            // 3. Tạo QuestionDto
            var questionDto = new QuestionDto
            {
                QuestionId = question.Id,
                Instruction = question.Instruction ?? string.Empty,
                VietnameseText = question.VietnameseText,
                EnglishText = question.EnglishText ?? string.Empty,
                Picture = question.Image,
                Audio = question.Audio,
                // Chuyển enum thành string
                Type = question.Type.ToString(),
                QuestionConfigure = question.QuestionConfiguration,
                OptionConfigure = question.OptionConfiguration,
                Order = question.Order,
                // Tạm thời để trống, lát nữa gán theo question.Type
                Options = new List<OptionDto>(),
                Words = question.Words.Select(w => new QuestionWordDto
                {
                    Word = w.Word.Content,
                    Order = w.Order,
                    Audio = w.Word.Audio
                }).ToList()
            };

            // 4. Tuỳ theo QuestionType, parse _options sang DTO
            switch (question.Type)
            {
                case QuestionType.MultipleChoice:
                {
                    // Lấy tất cả option thuộc kiểu MultipleChoiceQuestionOption
                    var mcOptions = question.Options
                        .OfType<MultipleChoiceQuestionOption>()
                        .Select(o => new MultipleChoiceOptionDto
                        {
                            // Gốc: o.Option là entity "Option"
                            OptionId = o.Option.Id,
                            EnglishText = o.Option.EnglishText ?? string.Empty,
                            VietnameseText = o.Option.VietnameseText ?? string.Empty,
                            Image = o.Option.Image,
                            Audio = o.Option.Audio,
                            IsCorrect = o.IsCorrect
                        })
                        .Cast<OptionDto>()
                        .ToList();

                    // Gán vào questionDto
                    questionDto.Options = mcOptions;
                    break;
                }

                case QuestionType.Matching:
                {
                    // Lấy tất cả option thuộc kiểu MatchingQuestionOption
                    var matchingOptions = question.Options
                        .OfType<MatchingQuestionOption>()
                        .Select(o => new MatchingOptionDto
                        {
                            // Gốc: o.Option là entity "Option"
                            OptionId = o.Option.Id,
                            EnglishText = o.Option.EnglishText ?? string.Empty,
                            VietnameseText = o.Option.VietnameseText ?? string.Empty,
                            Image = o.Option.Image,
                            Audio = o.Option.Audio,
                            TargetType = o.TargetType.ToString(),  // hoặc toString() if it's enum
                            SourceType = o.SourceType.ToString()
                        })
                        .Cast<OptionDto>()

                        .ToList();

                    questionDto.Options = matchingOptions;
                    break;
                }

                case QuestionType.Pronunciation:
                {
                    // Lấy tất cả option thuộc kiểu PronunciationQuestionOption
                    var pronunciationOptions = question.Options
                        .OfType<PronunciationQuestionOption>()
                        .Select(o => new PronunciationOptionDto
                        {
                            // Gốc: o.Option là entity "Option"
                            OptionId = o.Option.Id,
                            EnglishText = o.Option.EnglishText ?? string.Empty,
                            VietnameseText = o.Option.VietnameseText ?? string.Empty,
                            Image = o.Option.Image,
                            Audio = o.Option.Audio,
                        })
                        .Cast<OptionDto>()
                        .ToList();

                    questionDto.Options = pronunciationOptions;
                    break;
                }
                case QuestionType.BuildSentence:
                {
                    // Lấy tất cả option thuộc kiểu BuildSentenceQuestionOption
                    var buildSentenceOptions = question.Options
                        .OfType<BuildSentenceQuestionOption>()
                        .Select(o => new BuildSentenceOptionDto
                        {
                            // Gốc: o.Option là entity "Option"
                            OptionId = o.Option.Id,
                            EnglishText = o.Option.EnglishText ?? string.Empty,
                            VietnameseText = o.Option.VietnameseText ?? string.Empty,
                            Image = o.Option.Image, 
                            Audio = o.Option.Audio,
                            Order = o.Order,
                        })
                        .Cast<OptionDto>()
                        .ToList();

                    questionDto.Options = buildSentenceOptions;
                    break;  
                }
                        
                        
                default:
                {
                    // Nếu chưa handle type khác, ta để options rỗng
                    questionDto.Options = new List<OptionDto>();
                    break;
                }
            }

            // 5. Debug in ra console
            // PrintUtils.PrintAsJson(question);

            // 6. Trả về result
            return Result.Success(questionDto);
        }
    }
}

