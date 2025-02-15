using System;
using Microsoft.AspNetCore.Identity;
using SharedKernel;

namespace Infrastructure.Identity;

public static class IdentityResultExtensions
{
    public static Result ToApplicationResult(this IdentityResult result)
    {
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(result.Errors.Select(e => new Error(e.Description, e.Code, ErrorType.Validation)).First());
    }
}