using AgregaNews.Common.Contracts.QueueEvents;
using MassTransit;

namespace AgregaNews.AnalyzeNews.Application.Consumers.NewsAnalyze;

public sealed class NewsAnalyzeEventConsumer : IConsumer<NewsAnalyzeEvent>
{
    public Task Consume(ConsumeContext<NewsAnalyzeEvent> context)
    {
        var collectedMessage = context.Message;

        return Task.CompletedTask;
    }
}
