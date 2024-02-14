namespace AgregaNews.AnalyzeNews.Application.Responses;

public record AnalyzedNewsResponse(
    Guid Id,
    string? Author,
    string? Title,
    string Category,
    string? Description,
    string? Url,
    string? UrlToImage,
    DateTimeOffset? PublishedAt,
    string? Content,
    DateTimeOffset CreatedAt,
    bool Collected);
