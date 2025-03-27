using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interface;
using Application.Features.Media.Commands.Upload;
using Application.Interface;
using Domain.Entities.Learning.Words;
using Domain.Entities.Media;
using Domain.Entities.Media.Constants;
using Domain.Entities.Media.Enums;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Learning.Lessons.Commands.AddQuestions.Services
{
    public class WordGeneratorService : IWordGeneratorService
    {
        private readonly IWordService _wordService;
        private readonly IWordRepository _wordRepository;
        private readonly ITextToSpeechService _textToSpeechService;
        
        public WordGeneratorService(
            IWordService wordService,
            IWordRepository wordRepository,
            ITextToSpeechService textToSpeechService)
        {
            _wordService = wordService;
            _wordRepository = wordRepository;
            _textToSpeechService = textToSpeechService;
        }

        public async Task GenerateWords(string englishText)
        {
            if(string.IsNullOrEmpty(englishText)) return;

            var words = await _wordService.SplitWordsFromString(englishText);
            if(words.Count == 0) return;

            foreach (var word in words)
            {
                var existingWord = await _wordRepository.FindWord(word);
                if(existingWord != null) continue;
                Console.WriteLine($"Generating word: {word}");

                var uploadedAudio = await _textToSpeechService.GenerateMediaFromText(word, MediaConstants.Word);
                Domain.Entities.Media.Media? wordAudio = null;
                if(uploadedAudio.IsSuccess) wordAudio = uploadedAudio.Value;

                var newWord = Word.Create(word, wordAudio!);
                if(newWord.IsSuccess) await _wordRepository.AddWord(newWord.Value);

            }
        }
    }
}
