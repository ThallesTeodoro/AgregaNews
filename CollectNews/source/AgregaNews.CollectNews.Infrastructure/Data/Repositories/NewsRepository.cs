using AgregaNews.CollectNews.Domain.Contracts.Repositories;
using AgregaNews.CollectNews.Domain.Entities;
using MongoDB.Driver;

namespace AgregaNews.CollectNews.Infrastructure.Data.Repositories;

public class NewsRepository : Repository<News>, INewsRepository
{
    public NewsRepository(IMongoDatabase database)
        : base(database, nameof(News))
    {
    }
}
