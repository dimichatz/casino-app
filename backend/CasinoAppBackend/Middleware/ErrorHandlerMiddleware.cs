using System.Net;
using System.Text.Json;
using CasinoAppBackend.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CasinoAppBackend.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;
        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

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
                if (exception is SystemConfigurationException)
                {
                    _logger.LogCritical(exception, "Critical configuration error!");
                }
                else
                {
                    _logger.LogError(exception, "An unhandled exception occurred. Code: {Code}", (exception as AppException)?.Code);
                }

                var response = context.Response;
                response.ContentType = "application/json; charset=utf-8";

                response.StatusCode = exception switch
                {
                    EntityAlreadyExistsException or InvalidDataException 
                    or InvalidArgumentException or InsufficientBalanceException 
                    or DomainValidationException or BadRequestException
                    or MultipleErrorsException => (int)HttpStatusCode.BadRequest,                   // 400
                    EntityNotAuthorizedException => (int)HttpStatusCode.Unauthorized,               // 401
                    EntityForbiddenException => (int)HttpStatusCode.Forbidden,                      // 403
                    EntityNotFoundException => (int)HttpStatusCode.NotFound,                        // 404
                    DomainConflictException => (int)HttpStatusCode.Conflict,                        // 409
                    SystemConfigurationException => (int)HttpStatusCode.InternalServerError,        // 500
                    ExternalServiceFailedException
                    or ExternalServiceInvalidResponseException => (int)HttpStatusCode.BadGateway,   // 502
                    _ => (int)HttpStatusCode.InternalServerError                                    // 500
                };

                var problem = new ProblemDetails
                {
                    Type = exception.GetType().Name,
                    Title = exception.Message,
                    Status = response.StatusCode,
                    Detail = "See logs for more details.",
                    Instance = context.Request.Path
                };

                if (exception is AppException appException)
                    problem.Extensions["code"] = appException.Code;
                problem.Extensions["traceId"] = context.TraceIdentifier;
                problem.Extensions["timestamp"] = DateTime.UtcNow;

                if(exception is MultipleErrorsException multipleErrorsException)
                    problem.Extensions["errors"] = multipleErrorsException.Errors;

                var result = JsonSerializer.Serialize(problem, _options);

                await response.WriteAsync(result);
            }
        }
    }
}
