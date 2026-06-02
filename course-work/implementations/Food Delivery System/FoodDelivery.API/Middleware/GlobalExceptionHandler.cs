using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace FoodDelivery.API.Middleware
{
    public class GlobalExceptionHandler : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Грешка: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/problem+json";

            var (statusCode, title) = exception switch
            {
                KeyNotFoundException => (HttpStatusCode.NotFound, "Не е намерено"),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Неоторизиран достъп"),
                ArgumentException => (HttpStatusCode.BadRequest, "Невалидни данни"),
                InvalidOperationException => (HttpStatusCode.BadRequest, "Невалидна операция"),
                _ => (HttpStatusCode.InternalServerError, "Вътрешна грешка")
            };

            context.Response.StatusCode = (int)statusCode;

            var problem = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Detail = exception.Message,
                Instance = context.Request.Path
            };

            var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}