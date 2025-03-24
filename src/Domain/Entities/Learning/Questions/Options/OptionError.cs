using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedKernel;

namespace Domain.Entities.Learning.Questions.Options
{
    public class OptionError
    {
        public static Error InvalidEnglishSentenceOrWord() => Error.Validation(
            "Option_InvalidEnglishSentenceOrWord", 
            "English sentence or word is invalid");

        public static Error OptionNotFound => Error.NotFound(
            "OptionError.OptionNotFound", 
            "Option not found");

        public static Error OptionAlreadyExists() => Error.Conflict(
            "OptionError.OptionAlreadyExists", 
            "Option already exists");

        public static Error OptionAudioRequiresEnglishText() => Error.Conflict(
            "OptionError.OptionAudioRequiresEnglishText", 
            "Option audio requires English text");
    }
}