using AgregaNews.AnalyzeNews.Api.Contracts;

namespace AgregaNews.AnalyzeNews.Api.Middlewares;

internal sealed class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
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
            await HandlerExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandlerExceptionAsync(HttpContext httpContext, Exception ex)
    {
        (int statusCode, JsonResponse<object, object> response) statusCodeAndResponse = GetStatusCodeAndResponse(ex);

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = statusCodeAndResponse.statusCode;

        await httpContext.Response.WriteAsync(statusCodeAndResponse.response.ToString());
    }

    private (int statusCode, JsonResponse<object, object> response) GetStatusCodeAndResponse(Exception ex)
        => ex switch
        {
            _ => (StatusCodes.Status500InternalServerError, new JsonResponse<object, object>(StatusCodes.Status500InternalServerError, null, null))
        };
}
