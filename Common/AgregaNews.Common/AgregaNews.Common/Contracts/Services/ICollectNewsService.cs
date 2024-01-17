using AgregaNews.Common.DTOs;

namespace AgregaNews.Common.Contracts.Services;

public interface ICollectNewsService
{
    Task<NewsDto?> CollectTopHeadlinesAsync(string country, string? category, int? pageSize, int? page);
}
