using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interface;
using Application.Features.Media.Commands.Upload;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using WebApi.Extensions;
using WebApi.Infrastructure;

namespace WebApi.Controllers.Learning
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpeechConvertController : ControllerBase
    {
        private readonly ITextToSpeechService _textToSpeechService;
        private readonly IMediator _mediator;
        public SpeechConvertController(
            ITextToSpeechService textToSpeechService,
            IMediator mediator)
        {
            _textToSpeechService = textToSpeechService;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetSpeechFromText([FromQuery] string text)
        {      
            var audioBytes = _textToSpeechService.GenerateAudioFileFromText(text);
            if(audioBytes == null) return BadRequest();
            
            var uploadRequest = new MediaUploadRequest(
                string.Empty,
                audioBytes,
                text,
                "audio/mp3"
            );
            var uploadCommand = new MediaUploadCommand(uploadRequest);
            var uploadedFile = await _mediator.Send(uploadCommand);
            
            return uploadedFile.Match(Ok, CustomResults.Problem);


        }
    }
}