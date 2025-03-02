using System.IO;
using System.Threading.Tasks;

namespace Domain.Service
{
    public class SpeechToTextResult
    {
        public string Transcript { get; set; }
        public float Confidence { get; set; }
    }

    public class SpeechRecognitionConfig
    {
        public string LanguageCode { get; set; } = "en-US";
        public int SampleRateHertz { get; set; } = 16000;
        public string TargetWord { get; set; } = string.Empty;
    }

    public interface ISpeechToTextService
    {
        Task<SpeechToTextResult> RecognizeFromFileAsync(string audioFilePath, SpeechRecognitionConfig config);
        Task<SpeechToTextResult> RecognizeFromStreamAsync(Stream audioStream, SpeechRecognitionConfig config);
    }
}
