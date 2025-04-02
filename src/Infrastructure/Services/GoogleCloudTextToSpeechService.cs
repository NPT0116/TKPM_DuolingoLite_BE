
using System.Diagnostics;
using Application.Common.Interface;
using Application.Features.Media.Commands.Upload;
using Application.Interface;
using Domain.Entities.Media;
using Domain.Entities.Media.Constants;
using Domain.Entities.Media.Enums;
using Domain.Repositories;
using Google.Cloud.TextToSpeech.V1;
using MediatR;
using Microsoft.Extensions.Configuration;
using SharedKernel;

namespace Infrastructure.Services
{
    public class GoogleCloudTextToSpeechService : ITextToSpeechService
    {
        private readonly IConfiguration _configuration;
        private readonly IWordService _wordService;
        private readonly IMediaRepository _mediaRepository;
        private readonly IMediator _mediator;
        private readonly IApplicationDbContext _context;
        public GoogleCloudTextToSpeechService(
            IConfiguration configuration,
            IWordService wordService,
            IMediaRepository mediaRepository,
            IMediator mediator,
            IApplicationDbContext context)
        {
            _configuration = configuration;
            _wordService = wordService;
            _mediaRepository = mediaRepository;
            _mediator = mediator;
            _context = context;
        }

        public byte[] GenerateAudioFileFromText(string text)
        {
            if(string.IsNullOrEmpty(text)) return [];
            var _client = new TextToSpeechClientBuilder
            {
                CredentialsPath = _configuration["Google:FileCredentialsPath"]
            }.Build();
            var synthesisInput = new SynthesisInput
            {
                Text = text
            };

            var random = new Random();
            var genders = Enum.GetValues(typeof(SsmlVoiceGender));
            var randomGender = (SsmlVoiceGender)genders.GetValue(random.Next(genders.Length));
            
            VoiceSelectionParams voiceSelectionParams = new ()
            {
                LanguageCode = "en-US",
                SsmlGender = randomGender
            };

            var audioConfig = new AudioConfig
            {
                AudioEncoding = AudioEncoding.Mp3
            };

            var response = _client.SynthesizeSpeech(synthesisInput, voiceSelectionParams, audioConfig);
            var content = response.AudioContent;

            using (var memoryStream = new MemoryStream())
            {
                content.WriteTo(memoryStream);
                return memoryStream.ToArray();
            }

            // Process.Start(new ProcessStartInfo
            // {
            //     FileName = "audio.mp3",
            //     UseShellExecute = true
            // });
        }

        public async Task<Result<Media>> GenerateMediaFromText(string text, string? folder)
        {
            var wordDefinitions = await _wordService.GetWordDefinition(text);
            foreach(var wordDefinition in wordDefinitions)
            {
                var phonetics = wordDefinition.Phonetics;
                var phoneticWithAudio = phonetics.FirstOrDefault(p => !string.IsNullOrEmpty(p.Audio));
                if(phoneticWithAudio == null) continue;
                var audioUrl = phoneticWithAudio.Audio;
                var media = await _mediaRepository.UploadFileAsync(
                    text, audioUrl!, MediaType.Audio, 10,
                    DateTime.UtcNow, DateTime.UtcNow, audioUrl!, CancellationToken.None
                );
                if(media.IsSuccess)
                {
                    await _context.SaveChangesAsync();
                    return media;
                }
            }

            byte[] fileBytes = GenerateAudioFileFromText(text);
            var uploadRequest = new MediaUploadRequest(folder == null ? string.Empty : folder, fileBytes, text, MediaConstants.Audio);
            var command = new MediaUploadCommand(uploadRequest);
            var resultMedia = await _mediator.Send(command);
            return resultMedia;
        }
    }
}