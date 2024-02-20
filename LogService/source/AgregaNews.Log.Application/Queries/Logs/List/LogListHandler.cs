using AgregaNews.Log.Application.Responses;
using AgregaNews.Log.Domain.Contracts.Repositories;
using AutoMapper;
using MediatR;

namespace AgregaNews.Log.Application.Queries.Logs.List;

public sealed class LogListHandler : IRequestHandler<LogListQuery, PaginationResponse<LogResponse>>
{
    private readonly ILogRepository _logRepository;
    private readonly IMapper _mapper;

    public LogListHandler(ILogRepository logRepository, IMapper mapper)
    {
        _logRepository = logRepository;
        _mapper = mapper;
    }

    public async Task<PaginationResponse<LogResponse>> Handle(LogListQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page ?? 1;
        var pageSize = request.PageSize ?? 10;

        var pagination = await _logRepository.ListPaginateAsync(page, pageSize);

        return new PaginationResponse<LogResponse>()
        {
            CurrentPage = pagination.CurrentPage,
            Items = _mapper.Map<List<LogResponse>>(pagination.Items),
            Total = pagination.Total,
        };
    }
}
