using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Features.Learning.Lessons.Commands.AddQuestions;
using Domain.Entities.Learning.Questions.Enums;

namespace Application.Features.Learning.Lessons.Commands.UpdateQuestion;

public class UpdateQuestionDto : QuestionDto
{
    public UpdateQuestionDto(string? instruction, string? vietnameseText, string? englishText, string? image, string? audio, string? sentence, int order, QuestionType type, ConfigurationDto questionConfiguration, ConfigurationDto optionConfiguration, List<OptionBaseDto> options) : base(instruction, vietnameseText, englishText, image, audio, sentence, order, type, questionConfiguration, optionConfiguration, options)
    {
    }
}

public record UpdateQuestionCommand(Guid lessonId, int questionOrder, UpdateQuestionDto dto) : ICommand;