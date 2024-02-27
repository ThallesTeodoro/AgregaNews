using AgregaNews.AnalyzeNews.Api.Contracts;
using AgregaNews.AnalyzeNews.Infrastructure;
using AgregaNews.Common.Contracts.EventBus;
using AgregaNews.Common.Contracts.QueueEvents;
using AgregaNews.Common.Enums;

namespace AgregaNews.AnalyzeNews.Api.Middlewares;

internal sealed class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
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

    private async Task HandlerExceptionAsync(HttpContext httpContext, IEventBus eventBus, Exception ex)
    {
        (int statusCode, JsonResponse<object, object> response) statusCodeAndResponse = GetStatusCodeAndResponse(ex);

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = statusCodeAndResponse.statusCode;

        await eventBus.PublishAsync(new LogEvent()
        {
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
            Message = ex.Message,
            OccurredIn = DateTimeOffset.Now,
            Service = typeof(ExceptionHandlerMiddleware).Assembly.FullName ?? "",
            Severity = LogSeverityEnum.Error,
            ExceptionType = ex.GetType().ToString(),
            StackTrace = ex.StackTrace,
        });

        await httpContext.Response.WriteAsync(statusCodeAndResponse.response.ToString());
    }

    private (int statusCode, JsonResponse<object, object> response) GetStatusCodeAndResponse(Exception ex)
        => ex switch
        {
            _ => (StatusCodes.Status500InternalServerError, new JsonResponse<object, object>(StatusCodes.Status500InternalServerError, null, null))
        };
}
