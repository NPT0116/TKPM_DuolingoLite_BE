using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Learning.Options.Commands.CreateOption;
using Application.Features.Learning.Options.Queries.FindOptionByEnglish;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;
using WebApi.Infrastructure;

namespace WebApi.Controllers.Learning
{
    [ApiController]
    [Route("api/[controller]")]
    public class OptionController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OptionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetOptionsByEnglishText([FromQuery] string englishText)
        {
            var query = new FindOptionByEnglishQuery(englishText);
            var result = await _mediator.Send(query);
            return result.Match(Ok, CustomResults.Problem);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOption([FromBody] CreateOptionDto dto)
        {
            var command = new CreateOptionCommand(dto);
            var result = await _mediator.Send(command);
            return result.Match(Ok, CustomResults.Problem);
        }
    }
}