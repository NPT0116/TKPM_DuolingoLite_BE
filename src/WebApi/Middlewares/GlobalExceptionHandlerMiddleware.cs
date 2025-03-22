using System;
using System.Net;
using System.Text.Json;
using Application.Common.Utls;

namespace WebApi.Middlewares;

 public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // PrintUtils.PrintAsJson(exception);
            Console.WriteLine(exception);
            // Bạn có thể log exception tại đây nếu cần
            var response = new { error = exception.Message };
            PrintUtils.PrintAsJson(response);
            var result = JsonSerializer.Serialize(response);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(result);
        }
    }
