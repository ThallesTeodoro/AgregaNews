namespace AgregaNews.CollectNews.Application.Responses;

public record CollectNewsResponse(
    Guid Id,
    string? Author,
    string? Title,
    string? Description,
    string? Url,
    string? UrlToImage,
    DateTimeOffset? PublishedAt,
    string? Content);
