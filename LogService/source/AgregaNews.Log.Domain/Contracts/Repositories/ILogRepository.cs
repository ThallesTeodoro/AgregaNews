using AgregaNews.Log.Domain.DTOs;

namespace AgregaNews.Log.Domain.Contracts.Repositories;

public interface ILogRepository : IRepository<Entities.Log>
{
    Task<Pagination<Entities.Log>> ListPaginateAsync(int page, int pageSize);
}
