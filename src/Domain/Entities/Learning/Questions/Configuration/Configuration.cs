using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedKernel;

namespace Domain.Entities.Learning.Question.Configuration
{
    public class Configuration : Entity
    {
        public bool Audio { get; private set; }
        public bool EnglishText { get; private set; }
        public bool VietnameseText { get; private set; }
        public bool Instruction { get; private set; }
        public bool Image { get; private set; }

        private Configuration() { }

        private Configuration(bool audio, bool englishText, bool vietnameseText, bool instruction, bool image)
        {
            Audio = audio;
            EnglishText = englishText;
            VietnameseText = vietnameseText;
            Instruction = instruction;
            Image = image;
        }
        
        public static Result<Configuration> Create(
            bool audio,
            bool englishText,
            bool vietnameseText,
            bool instruction,
            bool image
        )
        {
            return Result.Success(new Configuration(audio, englishText, vietnameseText, instruction, image));
        }
    }
}