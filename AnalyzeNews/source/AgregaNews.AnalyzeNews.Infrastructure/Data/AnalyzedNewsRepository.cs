using AgregaNews.AnalyzeNews.Domain.Contracts.Repositories;
using AgregaNews.AnalyzeNews.Domain.Entities;
using MongoDB.Driver;

namespace AgregaNews.AnalyzeNews.Infrastructure.Data;

public class AnalyzedNewsRepository : Repository<AnalyzedNews>, IAnalyzedNewsRepository
{
    public AnalyzedNewsRepository(IMongoDatabase database) 
        : base(database, nameof(AnalyzedNews))
    {
    }

    public async Task<List<AnalyzedNews>> GetRecentAsync(int size)
    {
        return await _dbCollection
            .Find(Builders<AnalyzedNews>.Filter.Empty)
            .SortByDescending(a => a.CreatedAt)
            .Limit(size)
            .ToListAsync();
    }
}
