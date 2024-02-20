using AgregaNews.Log.Application.Queries.Logs.List;
using AgregaNews.Log.Application.Responses;
using AgregaNews.Log.LogApi.Contracts;
using Carter;
using MediatR;

namespace AgregaNews.Log.LogApi.Modules;

public class LogModule : CarterModule
{
    public LogModule()
        : base("/logs")
    {
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (
            [AsParameters] LogListParameter parameters,
            ISender sender,
            HttpContext httpContext) =>
        {
            var response = new JsonResponse<PaginationResponse<LogResponse>, List<object>>(StatusCodes.Status200OK, null, null);

            response.Data = await sender.Send(new LogListQuery(parameters.Page, parameters.PageSize));

            httpContext.Response.StatusCode = response.StatusCode;

            return response.ToString();
        })
        .WithName("Logs")
        .Produces<JsonResponse<PaginationResponse<LogResponse>, List<object>>>(StatusCodes.Status200OK)
        .Produces<JsonResponse<List<object>, List<object>>>(StatusCodes.Status500InternalServerError);
    }
}
