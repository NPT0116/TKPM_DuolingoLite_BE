using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Media;
using SharedKernel;

namespace Domain.Entities.Word
{
    public class Word : Entity
    {
        public string Content { get; private set; } = string.Empty;
        public Media.Media? Image { get; private set; }
        public Media.Media? Audio { get; private set; }

        private Word() {}

        private Word(
            string content,
            Media.Media? image,
            Media.Media? audio
        )
        {
            Content = content;
            Image = image;
            Audio = audio;
        }

        public static Result<Word> Create(
            string content, 
            Media.Media? image,
            Media.Media? audio)
        {
            if(string.IsNullOrEmpty(content))
            {
                return Result.Failure<Word>(WordError.EmptyContent());
            }

            var word = new Word(content.Trim(), image, audio);

            return Result.Success(word);
        }
    }
}