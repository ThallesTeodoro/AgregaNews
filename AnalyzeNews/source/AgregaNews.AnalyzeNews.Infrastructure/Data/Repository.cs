using AgregaNews.AnalyzeNews.Domain.Contracts.Repositories;
using AgregaNews.Common.Contracts.Data;
using MongoDB.Driver;

namespace AgregaNews.AnalyzeNews.Infrastructure.Data;

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
