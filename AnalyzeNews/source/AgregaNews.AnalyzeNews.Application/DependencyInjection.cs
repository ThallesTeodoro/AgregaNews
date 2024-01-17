using AgregaNews.AnalyzeNews.Application.Consumers.NewsAnalyze;
using AgregaNews.Common.Infrastructure.MessageBroker;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace AgregaNews.AnalyzeNews.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMassTransit(bussConfigurator =>
        {
            bussConfigurator.SetKebabCaseEndpointNameFormatter();

            bussConfigurator.AddConsumer<NewsAnalyzeEventConsumer>();

            bussConfigurator.UsingRabbitMq((context, configurator) =>
            {
                MessageBrokerSettings settings = context.GetRequiredService<MessageBrokerSettings>();

                configurator.Host(new Uri(settings.Host), h =>
                {
                    h.Username(settings.Username);
                    h.Password(settings.Password);
                });
            });
        });

        return services;
    }
}
