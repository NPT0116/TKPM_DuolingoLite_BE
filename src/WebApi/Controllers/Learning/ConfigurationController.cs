using Application.Features.Learning.Configures.Query.GetAConfigure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;
using WebApi.Infrastructure;

namespace WebApi.Controllers.Learning
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly IMediator  _mediator;
        public ConfigurationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("{configureId}")]
        public async Task<IActionResult> GetConfigureById([FromRoute] Guid configureId, CancellationToken cancellationToken = default)
        {
            var query = new GetAConfigureQuery(configureId);
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match(Ok, CustomResults.Problem);
        }
    }
}
