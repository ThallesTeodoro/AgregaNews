using AgregaNews.CollectNews.Application.Responses;
using MediatR;

namespace AgregaNews.CollectNews.Application.Queries.CollectNews.Collect;

public record CollectNewsQuery(string? Country, string? Category, int? PageSize, int? Page) : IRequest<List<CollectNewsResponse>>;

public record CollectNewsParameter(string? Country, string? Category, int? PageSize, int? Page);
