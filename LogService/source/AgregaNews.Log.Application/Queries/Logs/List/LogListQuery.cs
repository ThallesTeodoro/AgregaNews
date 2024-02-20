using AgregaNews.Log.Application.Responses;
using MediatR;

namespace AgregaNews.Log.Application.Queries.Logs.List;

public record LogListQuery(int? Page, int? PageSize) : IRequest<PaginationResponse<LogResponse>>;

public record LogListParameter(int? Page, int? PageSize);