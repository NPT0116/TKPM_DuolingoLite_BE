using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain.Service;
using Google.Cloud.Speech.V1;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class SpeechToTextService : ISpeechToTextService
    {
        private readonly IConfiguration _configuration;
        public SpeechToTextService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<SpeechToTextResult> RecognizeFromFileAsync(string audioFilePath, SpeechRecognitionConfig config)
        {
            // Tạo client của Google Speech-to-Text
            SpeechClient client = SpeechClient.Create();

            // Ánh xạ cấu hình từ domain sang cấu hình của Google
            RecognitionConfig googleConfig = MapToGoogleRecognitionConfig(config);

            // Tạo đối tượng audio từ file
            RecognitionAudio audio = RecognitionAudio.FromFile(audioFilePath);

            // Gọi API nhận dạng
            RecognizeResponse response = await client.RecognizeAsync(googleConfig, audio);

            // Lấy kết quả đầu tiên
            var result = response.Results.FirstOrDefault();
            if (result != null)
            {
                var alternative = result.Alternatives.FirstOrDefault();
                if (alternative != null)
                {
                    return new SpeechToTextResult
                    {
                        Transcript = alternative.Transcript,
                        Confidence = alternative.Confidence
                    };
                }
            }

            // Nếu không có kết quả, trả về kết quả rỗng
            return new SpeechToTextResult { Transcript = string.Empty, Confidence = 0 };
        }

        public async Task<SpeechToTextResult> RecognizeFromStreamAsync(Stream audioStream, SpeechRecognitionConfig config)
        {
            string credentialsPath = _configuration["Google:CredentialsPath"];
            var client = new SpeechClientBuilder
            {
                CredentialsPath = credentialsPath
            }.Build();
            RecognitionConfig googleConfig = MapToGoogleRecognitionConfig(config);

            // Chú ý: Nếu audioStream không hỗ trợ việc đọc lại từ đầu, hãy chuyển đổi thành MemoryStream
            RecognitionAudio audio = RecognitionAudio.FromStream(audioStream);

            RecognizeResponse response = await client.RecognizeAsync(googleConfig, audio);

            var result = response.Results.FirstOrDefault();
            if (result != null)
            {
                var alternative = result.Alternatives.FirstOrDefault();
                if (alternative != null)
                {
                    return new SpeechToTextResult
                    {
                        Transcript = alternative.Transcript,
                        Confidence = alternative.Confidence
                    };
                }
            }
            return new SpeechToTextResult { Transcript = string.Empty, Confidence = 0 };
        }

        /// <summary>
        /// Hàm ánh xạ cấu hình nhận dạng từ domain sang cấu hình của Google.
        /// Ở đây, nếu không có thuộc tính Encoding trong SpeechRecognitionConfig, ta mặc định dùng Linear16.
        /// </summary>
        private RecognitionConfig MapToGoogleRecognitionConfig(SpeechRecognitionConfig config)
        {
            return new RecognitionConfig
            {
                LanguageCode = config.LanguageCode,
                SampleRateHertz = config.SampleRateHertz,
                Encoding = RecognitionConfig.Types.AudioEncoding.Mp3
            };
        }
    }
}
