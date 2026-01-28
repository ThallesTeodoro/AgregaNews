using AgregaNews.CollectNews.Application.Responses;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace AgregaNews.CollectNews.Application.Queries.CollectNews.Collect;

public record CollectNewsQuery(string? Country, string? Category, int? PageSize, int? Page) : IRequest<List<CollectNewsResponse>>;

public record CollectNewsParameter(
    [Length(2, 2, ErrorMessage = "Country must be a 2-letter ISO code")]
    string? Country, 
    string? Category, 
    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
    int? PageSize, 
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    int? Page);
