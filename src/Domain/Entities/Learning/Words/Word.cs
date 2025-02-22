using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Media;
using SharedKernel;

namespace Domain.Entities.Learning.Words
{
    public class Word : Entity
    {
        public string Content { get; private set; } = string.Empty;
        public Media.Media Audio { get; private set; }

        private Word() {}

        private Word(
            string content,
            Media.Media audio
        )
        {
            Content = content;
            Audio = audio;
        }

        public static Result<Word> Create(
            string content, 
            Media.Media audio)
        {
            if(string.IsNullOrEmpty(content))
            {
                return Result.Failure<Word>(WordError.EmptyContent());
            }

            if(audio == null)
            {
                return Result.Failure<Word>(WordError.EmptyAudio());
            }

            var word = new Word(content.Trim(), audio);

            return Result.Success(word);
        }
    }
}