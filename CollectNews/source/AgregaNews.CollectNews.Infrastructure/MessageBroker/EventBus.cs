using AgregaNews.CollectNews.Domain.Contracts.EventBus;
using MassTransit;

namespace AgregaNews.CollectNews.Infrastructure.MessageBroker;

public class EventBus : IEventBus
{
    private readonly IPublishEndpoint _publishEndpoint;

    public EventBus(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : class =>
        _publishEndpoint.Publish(message, cancellationToken);

    public Task PublishAsync<T>(List<T> messages, CancellationToken cancellationToken = default) 
        where T : class
    {
        foreach (var message in messages)
        {
            _publishEndpoint.Publish(message, cancellationToken);
        }

        return Task.CompletedTask;
    }
}
