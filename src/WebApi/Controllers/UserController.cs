using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Heart.Commands.LoseHeart;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;
using WebApi.Infrastructure;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserController(IMediator mediator)
        {
            _mediator = mediator;            
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UserLoseHeart()
        {
            var command = new LoseHeartCommand();
            var result = await _mediator.Send(command);

            return result.Match(Ok, CustomResults.Problem);
        }
    }
}