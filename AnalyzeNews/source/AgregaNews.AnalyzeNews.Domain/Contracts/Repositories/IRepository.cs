using AgregaNews.Common.Contracts.Data;

namespace AgregaNews.AnalyzeNews.Domain.Contracts.Repositories;
public interface IRepository<T> where T : IEntity
{
    Task AddAsync(T entity);
}
