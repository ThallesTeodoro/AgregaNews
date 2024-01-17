using AgregaNews.CollectNews.Api.Contracts;
using AgregaNews.CollectNews.Application.Queries.CollectNews.Collect;
using AgregaNews.CollectNews.Application.Responses;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgregaNews.CollectNews.Api.Modules;

public class CollectNewsModule : CarterModule
{
    public CollectNewsModule()
        : base("/collect-news")
    {
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (
            [AsParameters] CollectNewsParameter parameters, 
            [FromServices] ISender sender,
            HttpContext httpContext) =>
        {
            var response = new JsonResponse<List<CollectNewsResponse>, List<object>>(StatusCodes.Status200OK, null, null);

            response.Data = await sender.Send(new CollectNewsQuery(
                parameters.Category,
                parameters.Country,
                parameters.Page,
                parameters.PageSize
            ));

            httpContext.Response.StatusCode = response.StatusCode;

            return response.ToString();
        })
        .WithName("CollectNews")
        .Produces<JsonResponse<List<CollectNewsResponse>, List<object>>>(StatusCodes.Status200OK)
        .Produces<JsonResponse<List<object>, List<object>>>(StatusCodes.Status500InternalServerError);
    }
}
