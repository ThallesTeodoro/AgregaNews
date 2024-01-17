namespace AgregaNews.Common.DTOs;

public record NewsDto(
    string status,
    int totalResults,
    List<ArticleDto> articles);