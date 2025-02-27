using System;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace WebApi.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult Match<T>(
            this Result<T> result,
            Func<Result<T>, IActionResult> onSuccess, // Chuyển từ Func<T, IActionResult> thành Func<Result<T>, IActionResult>
            Func<Result, IActionResult> onFailure)
        {
            return result.IsSuccess 
                ? onSuccess(result) // Trả về toàn bộ Result<T> thay vì chỉ result.Value
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
