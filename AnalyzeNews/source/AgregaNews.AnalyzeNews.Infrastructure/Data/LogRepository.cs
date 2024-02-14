using AgregaNews.AnalyzeNews.Domain.Contracts.Repositories;
using AgregaNews.AnalyzeNews.Domain.Entities;
using MongoDB.Driver;

namespace AgregaNews.AnalyzeNews.Infrastructure.Data;
public class LogRepository : Repository<Log>, ILogRepository
{
    public LogRepository(IMongoDatabase database) 
        : base(database, nameof(Log))
    {
    }
}
