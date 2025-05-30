using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedKernel;

namespace Domain.Entities.Learning.Words
{
    public class WordError
    {
        public static Error EmptyContent() => Error.Validation(
            "Word.EmptyContent",
            "Content of word can not be empty"
        );

        public static Error EmptyMeaning() => Error.Validation(
            "Word.EmptyMeaning",
            "Meaning of word can not be empty"
        );

        public static Error EmptyAudio() => Error.Validation(
            "Word.EmptyAudio",
            "Audio of word can not be empty"
        );
    }
}