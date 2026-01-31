using System.Net;
using System.Text.Json;
using HighLowGameApi.Exceptions;

namespace HighLowGameApi.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
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
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unhandled exception in HighLowGameApi");

                var response = context.Response;
                response.ContentType = "application/json; charset=utf-8";

                response.StatusCode = exception switch
                {
                    SessionNotActiveException _ => 403,
                    EntityNotFoundException _ => 404,
                    _ => 500
                };

                var result = JsonSerializer.Serialize(new
                {
                    error = exception.Message,
                    code = exception.GetType().Name
                });

                await response.WriteAsync(result);
            }
        }
    }
}
