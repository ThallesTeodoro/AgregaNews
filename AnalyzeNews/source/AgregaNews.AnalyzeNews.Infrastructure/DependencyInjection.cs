using AgregaNews.AnalyzeNews.Domain.Contracts.Services;
using AgregaNews.AnalyzeNews.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using AgregaNews.AnalyzeNews.Domain.Contracts.Repositories;
using AgregaNews.AnalyzeNews.Infrastructure.Data;
using AgregaNews.Common.Contracts.EventBus;
using AgregaNews.Common.Infrastructure.MessageBroker;

namespace AgregaNews.AnalyzeNews.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddTransient<IChatGPTService, ChatGPTService>();

        services.AddScoped<IAnalyzedNewsRepository, AnalyzedNewsRepository>();

        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

        services.AddTransient<IEventBus, EventBus>();

        return services;
    }
}
