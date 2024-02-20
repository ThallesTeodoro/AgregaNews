using AgregaNews.Common.Contracts.Data;

namespace AgregaNews.Log.Domain.Contracts.Repositories;

public interface IRepository<T> where T : IEntity
{
    Task AddAsync(T entity);
}

