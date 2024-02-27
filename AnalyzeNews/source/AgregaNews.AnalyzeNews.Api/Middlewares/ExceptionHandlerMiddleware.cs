using AgregaNews.AnalyzeNews.Api.Contracts;
using AgregaNews.Common.Contracts.EventBus;
using AgregaNews.Common.Contracts.QueueEvents;
using AgregaNews.Common.Enums;

namespace AgregaNews.AnalyzeNews.Api.Middlewares;

internal sealed class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext httpContext, IEventBus eventBus)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandlerExceptionAsync(httpContext, eventBus, ex);
        }
    }

    private async Task HandlerExceptionAsync(HttpContext httpContext, IEventBus eventBus, Exception exception)
    {
        (int statusCode, JsonResponse<object, object> response) statusCodeAndResponse = GetStatusCodeAndResponse(exception);

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = statusCodeAndResponse.statusCode;

        await eventBus.PublishAsync(new LogEvent()
        {
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
            Message = exception.Message,
            OccurredIn = DateTimeOffset.Now,
            Service = typeof(ExceptionHandlerMiddleware).Assembly.FullName ?? "",
            Severity = LogSeverityEnum.Error,
            ExceptionType = exception.GetType().ToString(),
            StackTrace = exception.StackTrace,
        });

        _logger.LogError(
            "Request failure {@Error}, {@DateTimeUtc}",
            exception.Message,
            DateTime.UtcNow);

        await httpContext.Response.WriteAsync(statusCodeAndResponse.response.ToString());
    }

    private (int statusCode, JsonResponse<object, object> response) GetStatusCodeAndResponse(Exception exception)
        => exception switch
        {
            _ => (StatusCodes.Status500InternalServerError, new JsonResponse<object, object>(StatusCodes.Status500InternalServerError, null, null))
        };
}
