﻿using System.Threading.Tasks;

namespace AgregaNews.CollectNews.Domain.Contracts.EventBus;

public interface IEventBus
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : class;

    Task PublishAsync<T>(List<T> messages, CancellationToken cancellationToken = default)
        where T : class;
}
