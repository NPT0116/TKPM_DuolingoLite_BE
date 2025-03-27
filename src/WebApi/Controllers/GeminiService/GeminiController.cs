using Amazon.S3.Model;
using Application.Common.Interface;
using Application.Interfaces;
using Domain.Entities.Learning.Words.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.GeminiService
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeminiController : ControllerBase
    {
        private readonly IWordService _aiService;

        public GeminiController(IWordService aiService)
        {
            _aiService = aiService;
        }
        public record WordsDto(List<string> Words);
        [HttpPost("split-words")]
        public async Task<IActionResult> SplitWords([FromBody] string prompt, [FromQuery] Language language)
        {
            var words = await _aiService.SplitWordsFromString(prompt, language);
            var wordsDto = new WordsDto(words);
            return Ok(wordsDto);
        }
    }
}
