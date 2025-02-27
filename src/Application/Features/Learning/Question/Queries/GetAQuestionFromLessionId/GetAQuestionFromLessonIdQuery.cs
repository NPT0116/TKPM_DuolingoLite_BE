using System;
using Application.Abstractions.Messaging;

namespace Application.Features.Learning.Question.Queries.GetAQuestionFromLessionId;


using System;
using System.Collections.Generic;
using Domain.Entities.Learning.Questions.Configurations;
using Domain.Entities.Media;

public class    QuestionDto
{
    public string Instruction { get; set; } = string.Empty;
    public Guid QuestionId { get; set; }
    public string? VietnameseText { get; set; }
    public Media? Picture { get; set; }
    public string EnglishText { get; set; } = string.Empty;
    public Media? Audio { get; set; }
    public string Type { get; set; } = string.Empty;
    public Configuration QuestionConfigure { get; set; }
    public Configuration OptionConfigure { get; set; }
    public int Order { get; set; }
    public List<OptionDto> Options { get; set; } = new();
    public List<QuestionWordDto> Words { get; set; } = new();
}


public abstract class OptionDto
{
    public Guid OptionId { get; set; }
    public string VietnameseText { get; set; } = string.Empty;
    public string EnglishText { get; set; } = string.Empty;
    public Media Image { get; set; }
    public Media Audio { get; set; }

    // Hàm constructor đảm bảo không có giá trị null
}

public class MatchingOptionDto : OptionDto
{
    public string TargetType { get; set; } = string.Empty; // EnglishText, Image, Audio, etc.
    public string SourceType { get; set; } = string.Empty; // VietNamText, etc.
}
public class PronunciationOptionDto : OptionDto
{
    public bool RequiresRecording { get; set; } = false; // Nếu cần người dùng ghi âm giọng nói
}
public class MultipleChoiceOptionDto : OptionDto
{
    public bool IsCorrect { get; set; } = false;
}
public class BuildSentenceOptionDto : OptionDto
{
    public int Order { get; set; } = 0; // Thứ tự xuất hiện trong câu
}
public class QuestionWordDto
{
    public string Word { get; set; } = string.Empty;
    public int Order { get; set; } = 0; // Thứ tự xuất hiện trong câu
    public Media Audio { get; set; }
}

public record GetAQuestionFromLessonIdQuery(Guid LessonId, int QuestionOrder) : IQuery<QuestionDto>;
