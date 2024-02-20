using AgregaNews.Log.Domain.Contracts.Repositories;
using AgregaNews.Log.Domain.DTOs;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace AgregaNews.Log.Infrastructure.Data;

public class LogRepository : Repository<Domain.Entities.Log>, ILogRepository
{
    public LogRepository(IMongoDatabase database) 
        : base(database, nameof(Domain.Entities.Log))
    {
    }

    public async Task<Pagination<Domain.Entities.Log>> ListPaginateAsync(int page, int pageSize)
    {
        var query = _dbCollection.AsQueryable();

        var total = query.CountAsync();
        var items = query
            .OrderByDescending(a => a.OccurredIn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        await Task.WhenAll(total, items);

        return new Pagination<Domain.Entities.Log>()
        {
            CurrentPage = page,
            Items = items.Result,
            Total = total.Result
        };
    }
}
