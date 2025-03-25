using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Learning.Lessons.Commands.AddQuestions.Services
{
    public interface IWordGeneratorService
    {
        Task GenerateWords(string englishText);
    }
}