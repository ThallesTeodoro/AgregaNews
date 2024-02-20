using AgregaNews.Log.Domain.Contracts.Repositories;
using AgregaNews.Log.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace AgregaNews.Log.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ILogRepository, LogRepository>();

        return services;
    }
}
