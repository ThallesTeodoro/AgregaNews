namespace AgregaNews.Log.LogApi.Middlewares;

public class ResponseContentTypeMiddleware
{
    private readonly RequestDelegate _next;

    public ResponseContentTypeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        httpContext.Response.ContentType = "application/json";

        await _next(httpContext);
    }
}
