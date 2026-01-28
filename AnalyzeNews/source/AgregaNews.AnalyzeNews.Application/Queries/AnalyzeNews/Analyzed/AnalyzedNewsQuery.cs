using AgregaNews.AnalyzeNews.Application.Responses;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace AgregaNews.AnalyzeNews.Application.Queries.AnalyzeNews.Analyzed;

public record AnalyzedNewsQuery(int? size) : IRequest<List<AnalyzedNewsResponse>>;

public record AnalyzedNewsParameter(
    [Range(1, 100, ErrorMessage = "Size must be between 1 and 100")]
    int? size);
