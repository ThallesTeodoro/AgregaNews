namespace AgregaNews.CollectNews.Domain.DTOs;

public record NewsDto(
    string status,
    int totalResults,
    List<ArticleDto> articles);