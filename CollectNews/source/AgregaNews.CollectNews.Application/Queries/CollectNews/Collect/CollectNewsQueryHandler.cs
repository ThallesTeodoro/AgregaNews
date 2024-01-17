using AgregaNews.CollectNews.Application.Responses;
using AgregaNews.Common.Contracts.EventBus;
using AgregaNews.Common.Contracts.QueueEvents;
using AgregaNews.Common.Contracts.Repositories;
using AgregaNews.Common.Contracts.Services;
using AgregaNews.Common.Entities;
using AutoMapper;
using MediatR;

namespace AgregaNews.CollectNews.Application.Queries.CollectNews.Collect;

public class CollectNewsQueryHandler : IRequestHandler<CollectNewsQuery, List<CollectNewsResponse>>
{
    private readonly ICollectNewsService _collectNewsService;
    private readonly IMapper _mapper;
    private readonly INewsRepository _newsRepository;
    private readonly IEventBus _eventBus;

    public CollectNewsQueryHandler(
        ICollectNewsService collectNewsService,
        IMapper mapper,
        INewsRepository newsRepository,
        IEventBus eventBus)
    {
        _collectNewsService = collectNewsService;
        _mapper = mapper;
        _newsRepository = newsRepository;
        _eventBus = eventBus;
    }

    public async Task<List<CollectNewsResponse>> Handle(CollectNewsQuery request, CancellationToken cancellationToken)
    {
        var result = await _collectNewsService.CollectTopHeadlinesAsync(request.Country ?? "br", request.Category, request.PageSize, request.Page);

        if (result is not null)
        {
            await _newsRepository.AddManyAsync(_mapper.Map<List<News>>(result.articles));
            await _eventBus.PublishAsync(_mapper.Map<List<NewsAnalyzeEvent>>(result.articles), cancellationToken);

            return _mapper.Map<List<CollectNewsResponse>>(result.articles);
        }

        return new List<CollectNewsResponse>();
    }
}
