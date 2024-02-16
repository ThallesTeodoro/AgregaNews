using AgregaNews.AnalyzeNews.Api.Contracts;
using AgregaNews.AnalyzeNews.Application.Queries.AnalyzeNews.Analyzed;
using AgregaNews.AnalyzeNews.Application.Responses;
using Carter;
using MediatR;

namespace AgregaNews.AnalyzeNews.Api.Modules;

public class AnalyzeNewsModule : CarterModule
{
    public AnalyzeNewsModule()
        : base("/analyze-news")
    {
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (
            [AsParameters] AnalyzedNewsParameter parameters,
            ISender sender, 
            HttpContext httpContext) =>
        {
            var response = new JsonResponse<List<AnalyzedNewsResponse>, List<object>>(StatusCodes.Status200OK, null, null);

            response.Data = await sender.Send(new AnalyzedNewsQuery(parameters.size));

            httpContext.Response.StatusCode = response.StatusCode;

            return response.ToString();
        })
        .WithName("AnalyzedNews")
        .Produces<JsonResponse<List<AnalyzedNewsResponse>, List<object>>>(StatusCodes.Status200OK)
        .Produces<JsonResponse<List<object>, List<object>>>(StatusCodes.Status500InternalServerError);
    }
}
