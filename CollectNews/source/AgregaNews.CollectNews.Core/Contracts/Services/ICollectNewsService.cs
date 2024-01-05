using AgregaNews.CollectNews.Domain.DTOs;

namespace AgregaNews.CollectNews.Domain.Contracts.Services;

public interface ICollectNewsService
{
    Task<NewsDto?> CollectTopHeadlines(string country, string? category, int? pageSize, int? page);
}
