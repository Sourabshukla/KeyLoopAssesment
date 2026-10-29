
using System.Diagnostics;

namespace KeyLoopSalesman.Api.Middleware;

public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate  _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next,ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var traceId = Activity.Current?.Id?? context.TraceIdentifier;

        context.Response.Headers["X-Trace-Id"] = traceId;

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["TraceId"] = traceId
        }))
        {
            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                var statusCode = context.Response.StatusCode;

                _logger.Log(
                    statusCode >= 500
                        ? LogLevel.Error
                        : statusCode >= 400
                            ? LogLevel.Warning
                            : LogLevel.Information,
                    "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs} ms",
                    context.Request.Method,
                    context.Request.Path.Value,
                    statusCode,
                    stopwatch.Elapsed.TotalMilliseconds);
            }
        }
    }
}
