using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Contracts.Requests
{
    public class ProfileUploadRequestDto
    {
        public IFormFile? Avatar { get; set; }
    }
}