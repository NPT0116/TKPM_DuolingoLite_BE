using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interface;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Learning
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpeechConvertController : ControllerBase
    {
        private readonly ITextToSpeechService _textToSpeechService;
        public SpeechConvertController(ITextToSpeechService textToSpeechService)
        {
            _textToSpeechService = textToSpeechService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSpeechFromText([FromQuery] string text)
        {
            Console.WriteLine("Run here");
            _textToSpeechService.GenerateAudioFileFromText(text);
            
            return Ok();
        }
    }
}