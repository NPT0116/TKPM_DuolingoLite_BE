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
    }
}