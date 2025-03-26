using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Domain.Entities.Learning.Questions.Configurations;
using Domain.Entities.Learning.Questions.Enums;
using Domain.Entities.Learning.Questions.QuestionOptions;

namespace Application.Features.Learning.Lessons.Commands.AddQuestions;

public class OptionBaseDto
{
    public int Order { get; set; }
    public Guid OptionId { get; set; }
    public bool? IsCorrect { get; set; }
    public MatchingQuestionOptionType? SourceType { get; set; }
    public MatchingQuestionOptionType? TargetType { get; set; }
    public int? Position { get; set; }

    public OptionBaseDto(
        int order,
        Guid optionId,
        bool? isCorrect,
        MatchingQuestionOptionType? sourceType,
        MatchingQuestionOptionType? targetType,
        int? position)
    {
        Order = order;
        OptionId = optionId;
        IsCorrect = isCorrect;
        SourceType = sourceType;
        TargetType = targetType;
        Position = position;
    }
}

// public class MultipleChoiceOptionBaseDto
// {
//     public bool IsCorrect { get; set; }
//     public MultipleChoiceOptionBaseDto(int order, Guid optionId, bool isCorrect) : base(order, optionId, isCorrect)
//     {
//         IsCorrect = isCorrect;
//     }
// }

public class ConfigurationDto
{
    public bool Audio { get; set; }
    public bool EnglishText { get; set; }
    public bool VietnameseText { get; set; }
    public bool Instruction { get; set; }
    public bool Image { get; set; }

    public ConfigurationDto(
        bool audio,
        bool englishText,
        bool vietnameseText,
        bool instruction,
        bool image)
    {
        Audio = audio;
        EnglishText = englishText;
        VietnameseText = vietnameseText;
        Instruction = instruction;
        Image = image;
    }
}

public class QuestionDto
{
    public string? Instruction { get; set; }
    public string? VietnameseText { get; set; }
    public string? EnglishText { get; set; }
    public string? Image { get; set; }
    public string? Audio { get; set; }
    public int Order { get; set; }
    public QuestionType Type { get; set; }
    public ConfigurationDto QuestionConfiguration { get; set; }
    public ConfigurationDto OptionConfiguration { get; set; }
    public List<OptionBaseDto> Options { get; set; }

    public QuestionDto(
        string? instruction,
        string? vietnameseText,
        string? englishText,
        string? image,
        string? audio,
        int order,
        QuestionType type,
        ConfigurationDto questionConfiguration,
        ConfigurationDto optionConfiguration,
        List<OptionBaseDto> options)
    {
        Instruction = instruction;
        VietnameseText = vietnameseText;
        EnglishText = englishText;
        Image = image;
        Audio = audio;
        Order = order;
        Type = type;
        QuestionConfiguration = questionConfiguration;
        OptionConfiguration = optionConfiguration;
        Options = options;
    }

    // Custom deconstruct method for tuple deconstruction
    public void Deconstruct(
        out string? instruction,
        out string? vietnameseText,
        out string? englishText,
        out string? image,
        out string? audio,
        out int order,
        out QuestionType type,
        out ConfigurationDto questionConfiguration,
        out ConfigurationDto optionConfiguration,
        out List<OptionBaseDto> options)
    {
        instruction = Instruction;
        vietnameseText = VietnameseText;
        englishText = EnglishText;
        image = Image;
        audio = Audio;
        order = Order;
        type = Type;
        questionConfiguration = QuestionConfiguration;
        optionConfiguration = OptionConfiguration;
        options = Options;
    }
}

public record AddQuestionCommand(Guid lessonId, QuestionDto question) : ICommand;