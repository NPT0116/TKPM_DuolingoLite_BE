using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Application.Features.Media.Commands.Upload;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Media
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly IMediator _mediator;
        public MediaController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFileAsync(
            IFormFile file,
            string? prefix
        )
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var request = new MediaUploadRequest(
                prefix ?? "",
                memoryStream.ToArray(),
                file.FileName,
                file.ContentType
            );

            var command = new MediaUploadCommand(request);
            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}