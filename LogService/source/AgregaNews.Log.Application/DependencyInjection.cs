using AgregaNews.Common.Infrastructure.MessageBroker;
using AgregaNews.Log.Application.Consumers.Logs;
using AgregaNews.Log.Application.Mappers;
using AutoMapper;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace AgregaNews.Log.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMassTransit(bussConfigurator =>
        {
            bussConfigurator.SetKebabCaseEndpointNameFormatter();

            bussConfigurator.AddConsumer<LogEventConsumer>();

            bussConfigurator.UsingRabbitMq((context, configurator) =>
            {
                MessageBrokerSettings settings = context.GetRequiredService<MessageBrokerSettings>();

                configurator.Host(new Uri(settings.Host), h =>
                {
                    h.Username(settings.Username);
                    h.Password(settings.Password);
                });

                configurator.ConfigureEndpoints(context);
            });
        });

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(ApplicationAssemblyReference.Assembly);
        });

        var autoMapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<LogProfile>();
        });

        var mapper = autoMapperConfiguration.CreateMapper();
        services.AddSingleton(mapper);

        return services;
    }
}
