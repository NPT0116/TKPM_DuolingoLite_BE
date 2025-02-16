using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedKernel;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult Match<T>(
            this Result<T> result,
            Func<T, IActionResult> onSuccess,
            Func<Result, IActionResult> onFailure)
        {
            return result.IsSuccess 
                ? onSuccess(result.Value) 
                : onFailure(result);
        }

        public static IActionResult Match(
            this Result result,
            Func<IActionResult> onSuccess,
            Func<Result, IActionResult> onFailure)
        {
            return result.IsSuccess 
                ? onSuccess() 
                : onFailure(result);
        }
    }
}