using AgregaNews.Common.Contracts.Data;
using AgregaNews.Log.Domain.Contracts.Repositories;
using MongoDB.Driver;

namespace AgregaNews.Log.Infrastructure.Data;

public class Repository<T> : IRepository<T> where T : IEntity
{
    protected readonly IMongoCollection<T> _dbCollection;

    public Repository(IMongoDatabase database, string collectionName)
    {
        _dbCollection = database.GetCollection<T>(collectionName);
    }

    public async Task AddAsync(T entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(T));
        }

        await _dbCollection.InsertOneAsync(entity);
    }
}
