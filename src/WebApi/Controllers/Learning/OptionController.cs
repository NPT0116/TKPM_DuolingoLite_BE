using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}