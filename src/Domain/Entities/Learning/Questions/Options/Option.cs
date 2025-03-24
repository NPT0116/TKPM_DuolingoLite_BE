using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedKernel;

namespace Domain.Entities.Learning.Questions.Options
{
    public class Option : Entity
    {
        public string? VietnameseText { get; private set; }
        public Media.Media? Image { get; private set; }
        public Media.Media? Audio { get; private set; }
        public string? EnglishText { get; private set; }            

        private Option() { }

        private Option(string? vietnameseText, Media.Media? image, Media.Media? audio, string? englishText)
        {
            VietnameseText = vietnameseText;
            Image = image;
            Audio = audio;
            EnglishText = englishText;
        }

        public static Result<Option> Create(string vietnameseText, Media.Media? image, Media.Media? audio, string? englishText)
        {
            // if (englishText != null)
            // {
            //     return Result.Failure<Option>(OptionError.InvalidEnglishSentenceOrWord());
            // }

            // if (audio != null && englishText != null)
            // {
            //     return Result.Failure<Option>(OptionError.InvalidEnglishSentenceOrWord());
            // }

            return Result.Success(new Option(vietnameseText, image, audio, englishText));
        }        
    }
}