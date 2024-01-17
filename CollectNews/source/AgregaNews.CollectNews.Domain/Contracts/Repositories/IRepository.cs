using AgregaNews.Common.Contracts.Data;

namespace AgregaNews.CollectNews.Domain.Contracts.Repositories;

public interface IRepository<T> where T : IEntity
{
    Task AddManyAsync(List<T> entities);
}
