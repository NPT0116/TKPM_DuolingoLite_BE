using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interface;
using Application.Features.Learning.Lessons.Queries.GetListOfLessonFromCourseId;
using Application.Features.Media.Commands.Upload;
using Application.Interface;
using Domain.Entities.Learning.Questions.QuestionOptions;
using Domain.Entities.Learning.Words;
using Domain.Entities.Media;
using Domain.Entities.Media.Constants;
using Domain.Entities.Media.Enums;
using Domain.Repositories;
using MediatR;
using SharedKernel;

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

        public async Task<Result<List<QuestionWord>>> GenerateWords(
            Domain.Entities.Learning.Questions.Question question,
            string englishText)
        {
            var questionWords = new List<QuestionWord>();
            if(string.IsNullOrEmpty(englishText)) return Result.Failure<List<QuestionWord>>(QuestionWordsError.EmptyEnglishText);

            var words = await _wordService.SplitWordsFromString(englishText);
            if(words.Count == 0) return Result.Failure<List<QuestionWord>>(QuestionWordsError.EmptyEnglishText);

            var order = 1;
            foreach (var word in words)
            {
                var existingWord = await _wordRepository.FindWord(word);
                QuestionWord? questionWord = null;
                if(existingWord != null)
                {
                    var createQuestionWord = QuestionWord.Create(existingWord, question, order++);
                    if(createQuestionWord.IsFailure) return Result.Failure<List<QuestionWord>>(createQuestionWord.Error);
                    questionWord = createQuestionWord.Value;
                }
                else
                {
                    Console.WriteLine($"Generating word: {word}");

                    var uploadedAudio = await _textToSpeechService.GenerateMediaFromText(word, MediaConstants.Word);
                    Domain.Entities.Media.Media? wordAudio = null;
                    if(uploadedAudio.IsSuccess) wordAudio = uploadedAudio.Value;

                    var newWord = Word.Create(word, wordAudio!);
                    if(newWord.IsFailure) return Result.Failure<List<QuestionWord>>(newWord.Error);
                    await _wordRepository.AddWord(newWord.Value);

                    var createQuestionWord = QuestionWord.Create(newWord.Value, question, order++);
                    if(createQuestionWord.IsFailure) return Result.Failure<List<QuestionWord>>(createQuestionWord.Error);
                    questionWord = createQuestionWord.Value;
                }

                if(questionWord != null) questionWords.Add(questionWord);
            }

            return Result.Success(questionWords);
        }
    }
}
