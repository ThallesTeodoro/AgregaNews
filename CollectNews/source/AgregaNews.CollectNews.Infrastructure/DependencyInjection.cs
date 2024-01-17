using AgregaNews.Common.Contracts.EventBus;
using AgregaNews.CollectNews.Infrastructure.Data.Repositories;
using AgregaNews.CollectNews.Infrastructure.Services;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using AgregaNews.Common.Infrastructure.MessageBroker;
using AgregaNews.CollectNews.Domain.Contracts.Services;
using AgregaNews.CollectNews.Domain.Contracts.Repositories;

namespace AgregaNews.CollectNews.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

        services.AddTransient<ICollectNewsService, CollectNewsService>();
        services.AddScoped<INewsRepository, NewsRepository>();

        services.AddMassTransit(bussConfigurator =>
        {
            bussConfigurator.SetKebabCaseEndpointNameFormatter();

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

        services.AddTransient<IEventBus, EventBus>();

        return services;
    }
}
