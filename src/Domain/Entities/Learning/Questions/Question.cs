using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Media;
using Domain.Entities.Learning.Questions.QuestionOptions;
using SharedKernel;
using Domain.Entities.Learning.Questions.Enums;
using Domain.Entities.Learning.Questions.Configurations;

namespace Domain.Entities.Learning.Questions
{
    public class Question : Entity
    {
        public string? Instruction { get; private set; } = string.Empty;
        public string? VietnameseText { get; private set; } = string.Empty;
        public Media.Media? Audio { get; private set; }
        public string? EnglishText { get; private set; }
        public QuestionType Type { get; private set; }
        public Media.Media? Image { get; private set; }
        public Configuration QuestionConfiguration { get; private set; }
        public Configuration OptionConfiguration { get; private set; }
        public int Order { get; private set; }

        private readonly List<QuestionOptionBase> _options = new();
        public IReadOnlyList<QuestionOptionBase> Options => _options.AsReadOnly();

        private readonly List<QuestionWord> _words = new();
        public IReadOnlyList<QuestionWord> Words => _words.AsReadOnly();

        private Question() {}

        private Question(
            string? instruction,
            string? vietnameseText,
            Media.Media? audio,
            string? englishText,
            Media.Media? image,
            QuestionType type,
            Configuration questionConfiguration,
            Configuration optionConfiguration,
            int order
        )
        {
            Instruction = instruction;
            VietnameseText = vietnameseText;
            Audio = audio;
            EnglishText = englishText;
            Type = type;
            Image = image;
            QuestionConfiguration = questionConfiguration;
            OptionConfiguration = optionConfiguration;
            Order = order;
        }

        public static Result<Question> Create(
            string? instruction,
            string? vietnameseText,
            Media.Media? audio,
            string? englishText,
            Media.Media? image,
            QuestionType type,
            Configuration questionConfiguration,
            Configuration optionConfiguration,
            int order
        )
        {
            if(
                string.IsNullOrEmpty(instruction) 
                && string.IsNullOrEmpty(vietnameseText) 
                && audio == null 
                && string.IsNullOrEmpty(englishText)
                && type != QuestionType.Matching)
            {
                return Result.Failure<Question>(QuestionError.AllPromptsNull());
            }
            return new Question(instruction, vietnameseText, audio, englishText, image, type, questionConfiguration, optionConfiguration, order);
        }

        public void AddOption(QuestionOptionBase option)
        {
            if (!IsValidOptionType(option))
            {
                throw new InvalidOperationException($"Option type does not match question type {Type}");
            }
            _options.Add(option);
        }

        public Result AddOptions(List<QuestionOptionBase> options)
        {
            if(!options.Any()) return Result.Failure(QuestionOptionError.NoOptions);
            foreach(var option in options)
            {
                if(!IsValidOptionType(option)) return Result.Failure(QuestionOptionError.QuestionTypeNotSupported);
            }
            _options.AddRange(options);
            return Result.Success();
        }

        private bool IsValidOptionType(QuestionOptionBase option)
        {
            return Type switch
            {
                QuestionType.MultipleChoice => option is MultipleChoiceQuestionOption,
                QuestionType.Matching => option is MatchingQuestionOption,
                QuestionType.BuildSentence => option is BuildSentenceQuestionOption,
                QuestionType.Pronunciation => option is PronunciationQuestionOption,
                _ => false
            };
        }

        public void AddWord(QuestionWord word)
        {
            _words.Add(word);
        }
    }
}