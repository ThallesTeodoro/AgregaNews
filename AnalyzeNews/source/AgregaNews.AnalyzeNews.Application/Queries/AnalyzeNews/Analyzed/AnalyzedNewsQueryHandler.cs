using AgregaNews.AnalyzeNews.Application.Responses;
using AgregaNews.AnalyzeNews.Domain.Contracts.Repositories;
using AutoMapper;
using MediatR;

namespace AgregaNews.AnalyzeNews.Application.Queries.AnalyzeNews.Analyzed;
public sealed class AnalyzedNewsQueryHandler : IRequestHandler<AnalyzedNewsQuery, List<AnalyzedNewsResponse>>
{
    private readonly IAnalyzedNewsRepository _analyzedNewsRepository;
    private readonly IMapper _mapper;

    public AnalyzedNewsQueryHandler(IAnalyzedNewsRepository analyzedNewsRepository, IMapper mapper)
    {
        _analyzedNewsRepository = analyzedNewsRepository;
        _mapper = mapper;
    }

    public async Task<List<AnalyzedNewsResponse>> Handle(AnalyzedNewsQuery request, CancellationToken cancellationToken)
    {
        var analyzedNews = await _analyzedNewsRepository.GetRecentAsync(request.size ?? 10);

        return _mapper.Map<List<AnalyzedNewsResponse>>(analyzedNews);
    }
}
