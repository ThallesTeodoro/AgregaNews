using AgregaNews.CollectNews.Domain.Contracts.Repositories;
using AgregaNews.Common.Contracts.Data;
using MongoDB.Driver;

namespace AgregaNews.CollectNews.Infrastructure.Data.Repositories;

public class Repository<T> : IRepository<T> where T : IEntity
{
    private readonly IMongoCollection<T> _dbCollection;

    public Repository(IMongoDatabase database, string collectionName)
    {
        _dbCollection = database.GetCollection<T>(collectionName);
    }

    public async Task AddManyAsync(List<T> entities)
    {
        if (entities is null)
        {
            throw new ArgumentNullException(nameof(T));
        }

        await _dbCollection.InsertManyAsync(entities);
    }
}
