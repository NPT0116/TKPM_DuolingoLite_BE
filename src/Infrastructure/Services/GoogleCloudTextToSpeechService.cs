
using System.Diagnostics;
using Application.Common.Interface;
using Google.Cloud.TextToSpeech.V1;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class GoogleCloudTextToSpeechService : ITextToSpeechService
    {
        private readonly IConfiguration _configuration;
        public GoogleCloudTextToSpeechService(IConfiguration configuration)
        {
            _configuration = configuration;
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
    }
}