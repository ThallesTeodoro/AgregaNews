using AgregaNews.AnalyzeNews.Domain.Entities;

namespace AgregaNews.AnalyzeNews.Domain.Contracts.Repositories;

public interface IAnalyzedNewsRepository : IRepository<AnalyzedNews>
{
    Task<List<AnalyzedNews>> GetRecentAsync(int size);
}
