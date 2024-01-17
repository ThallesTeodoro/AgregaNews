using AgregaNews.Common.Contracts.Repositories;
using AgregaNews.Common.Entities;
using MongoDB.Driver;

namespace AgregaNews.CollectNews.Infrastructure.Data.Repositories;

public class NewsRepository : Repository<News>, INewsRepository
{
    public NewsRepository(IMongoDatabase database)
        : base(database, nameof(News))
    {
    }
}
