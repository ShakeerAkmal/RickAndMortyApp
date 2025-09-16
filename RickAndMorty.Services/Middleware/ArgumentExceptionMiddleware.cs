using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace RickAndMorty.Services.Middleware
{
    public class ArgumentExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ArgumentExceptionMiddleware> _logger;

        public ArgumentExceptionMiddleware(RequestDelegate next, ILogger<ArgumentExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "ArgumentException caught: {Message}", ex.Message);
                await HandleArgumentExceptionAsync(context, ex);
            }
        }

        private static async Task HandleArgumentExceptionAsync(HttpContext context, ArgumentException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var response = new
            {
                error = "Bad Request",
                message = ex.Message,
                parameterName = ex.ParamName
            };

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }

    public static class ArgumentExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseArgumentExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ArgumentExceptionMiddleware>();
        }
    }
}