using AgregaNews.Common.Contracts.Data;

namespace AgregaNews.Common.Contracts.Repositories;

public interface IRepository<T> where T : IEntity
{
    Task AddManyAsync(List<T> entities);
}
