
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace KeyLoopSalesman.Api.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,Exception exception,CancellationToken cancellationToken)
    {
        var statusCode = exception is ArgumentException ? StatusCodes.Status400BadRequest : StatusCodes.Status500InternalServerError; 
        if (statusCode == 500)
        {
            _logger.LogError(exception,"Unhandled exception for request {TraceId}",httpContext.TraceIdentifier);
        }
        else
        {
            _logger.LogWarning("Request validation failed. TraceId: {TraceId}",httpContext.TraceIdentifier);
        }
        var response = new ProblemDetails
        {
            Status = statusCode,
            Title = statusCode == 400 ? "Invalid request" : "Internal server error",
            Detail = statusCode == 400 ? exception.Message : "An unexpected error occurred.",
            Instance = httpContext.Request.Path.Value
        };
        response.Extensions["traceId"] = httpContext.TraceIdentifier;
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType ="application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}
