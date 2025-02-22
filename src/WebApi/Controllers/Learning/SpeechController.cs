using Domain.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Learning
{
    [Route("api/[controller]")]
    [ApiController]
public class SpeechController : ControllerBase
    {
        private readonly ISpeechToTextService _speechService;

        public SpeechController(ISpeechToTextService speechService)
        {
            _speechService = speechService;
        }

        /// <summary>
        /// Upload file audio và trả về transcript dạng text.
        /// </summary>
        /// <param name="audioFile">File audio được upload từ form-data</param>
        /// <returns>Transcript và thông tin liên quan</returns>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadAudio( IFormFile audioFile)
        {
            if (audioFile == null || audioFile.Length == 0)
            {
                return BadRequest("Không có file audio được gửi lên.");
            }

            // Thiết lập cấu hình nhận dạng, có thể điều chỉnh theo nhu cầu hoặc lấy từ cấu hình
            var recognitionConfig = new SpeechRecognitionConfig
            {
                LanguageCode = "en-US",
                SampleRateHertz = 16000,
            };

            // Sử dụng phương thức RecognizeFromStreamAsync của dịch vụ
            using (var stream = audioFile.OpenReadStream())
            {
                var result = await _speechService.RecognizeFromStreamAsync(stream, recognitionConfig);
                return Ok(result);
            }
        }
    }
}
