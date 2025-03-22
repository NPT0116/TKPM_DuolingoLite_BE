using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Learning.Words.Queries.GetWordDefinition;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Learning
{
    [ApiController]
    [Route("api/[controller]")]
    public class DictionaryController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DictionaryController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetWordDefinition([FromQuery] string word)
        {
            var definitions = await _mediator.Send(new GetWordDefinitionQuery(word));
            if(definitions == null || definitions.Count == 0)
            {
                return NotFound();
            }
            return Ok(definitions);
        }
    }
}