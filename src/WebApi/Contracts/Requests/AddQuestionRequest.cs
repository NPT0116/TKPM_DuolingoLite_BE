using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Questions.Enums;

namespace WebApi.Contracts.Requests
{
    public class OptionBaseDto
{
    public int Order { get; set; }
    public string? VietnameseText { get; set; }
    public string? Image { get; set; }
    public string? Audio { get; set; }
    public string? EnglishText { get; set; }

    public OptionBaseDto(int order, string? vietnameseText, string? image, string? audio, string? englishText)
    {
        Order = order;
        VietnameseText = vietnameseText;
        Image = image;
        Audio = audio;
        EnglishText = englishText;
    }
}

public class MultipleChoiceOptionDto : OptionBaseDto
{
    public bool IsCorrect { get; set; }

    public MultipleChoiceOptionDto(
        int order,
        string? vietnameseText,
        string? image,
        string? audio,
        string? englishText,
        bool isCorrect)
        : base(order, vietnameseText, image, audio, englishText)
    {
        IsCorrect = isCorrect;
    }
}

public class ConfigurationDto
{
    public bool Audio { get; set; }
    public bool EnglishText { get; set; }
    public bool VietnameseText { get; set; }
    public bool Instruction { get; set; }
    public bool Image { get; set; }

    public ConfigurationDto(bool audio, bool englishText, bool vietnameseText, bool instruction, bool image)
    {
        Audio = audio;
        EnglishText = englishText;
        VietnameseText = vietnameseText;
        Instruction = instruction;
        Image = image;
    }
}



public class AddQuestionRequest
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

    public AddQuestionRequest(
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
}
}