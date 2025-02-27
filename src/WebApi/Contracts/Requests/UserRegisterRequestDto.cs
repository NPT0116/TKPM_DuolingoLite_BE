using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.User.Commands.Register;

namespace WebApi.Contracts.Requests
{
    public class UserRegisterRequestDto
    {
        public required UserRegisterDto UserRegisterDto { get; set; }
        public IFormFile? Avatar { get; set; }
    }
}