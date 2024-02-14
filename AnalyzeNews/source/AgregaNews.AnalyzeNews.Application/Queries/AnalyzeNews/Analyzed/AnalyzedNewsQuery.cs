using AgregaNews.AnalyzeNews.Application.Responses;
using MediatR;

namespace AgregaNews.AnalyzeNews.Application.Queries.AnalyzeNews.Analyzed;

public record AnalyzedNewsQuery(int? size) : IRequest<List<AnalyzedNewsResponse>>;

public record AnalyzedNewsParameter(int? size);
