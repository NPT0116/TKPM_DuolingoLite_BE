using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interface;
using Application.Features.Media.Commands.Upload;
using Application.Interface;
using Domain.Entities.Learning.Words;
using Domain.Entities.Media.Enums;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Learning.Lessons.Commands.AddQuestions.Services
{
    public class WordGeneratorService : IWordGeneratorService
    {
        private readonly IWordService _wordService;
        private readonly IWordRepository _wordRepository;
        private readonly IMediaRepository _mediaRepository;
        private readonly ITextToSpeechService _textToSpeechService;
        private readonly IMediator _mediator;
        private readonly IApplicationDbContext _context;
        
        public WordGeneratorService(
            IWordService wordService,
            IWordRepository wordRepository,
            IMediaRepository mediaRepository,
            ITextToSpeechService textToSpeechService,
            IMediator mediator,
            IApplicationDbContext context)
        {
            _wordService = wordService;
            _wordRepository = wordRepository;
            _mediaRepository = mediaRepository;
            _textToSpeechService = textToSpeechService;
            _mediator = mediator;
            _context = context;
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
                
                string audio = null;
                var wordDefinitions = await _wordService.GetWordDefinition(word);
                if(wordDefinitions == null) continue;
                foreach(var wordDefinition in wordDefinitions)
                {
                    var phonetics = wordDefinition.Phonetics;
                    var phoneticWithAudio = phonetics.FirstOrDefault(p => p.Audio != null);
                    if(phoneticWithAudio == null) continue;
                    audio = phoneticWithAudio.Audio!;
                    break;
                }

                Domain.Entities.Media.Media? wordAudio = null;

                if(audio != null)
                {
                    var uploadedWordAudio = await _mediaRepository.UploadFileAsync(audio, audio, MediaType.Audio, 10, DateTime.UtcNow, DateTime.UtcNow, audio, CancellationToken.None);
                    if(uploadedWordAudio.IsSuccess)
                    {
                        await _context.SaveChangesAsync();
                        wordAudio = uploadedWordAudio.Value;
                    }
                }
                else
                {
                    var audioBytes = _textToSpeechService.GenerateAudioFileFromText(word);
                    var uploadRequest = new MediaUploadRequest(
                        string.Empty,
                        audioBytes,
                        word,
                        "audio/mp3"
                    );
                    var uploadCommand = new MediaUploadCommand(uploadRequest);
                    var uploadedWordAudio = await _mediator.Send(uploadCommand);
                    if(uploadedWordAudio.IsFailure) continue;
                    await _context.SaveChangesAsync();
                    wordAudio = uploadedWordAudio.Value;
                }

                if(wordAudio == null) continue;

                var newWord = Word.Create(word, wordAudio!);
                if(newWord.IsSuccess) await _wordRepository.AddWord(newWord.Value);

            }
        }
    }
}
