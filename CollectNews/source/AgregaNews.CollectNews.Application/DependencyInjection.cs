using AgregaNews.CollectNews.Application.Mappers;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace AgregaNews.CollectNews.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(ApplicationAssemblyReference.Assembly);
        });

        var automapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CollectNewsProfile>();
        });

        var mapper = automapperConfiguration.CreateMapper();
        services.AddSingleton(mapper);

        return services;
    }
}
