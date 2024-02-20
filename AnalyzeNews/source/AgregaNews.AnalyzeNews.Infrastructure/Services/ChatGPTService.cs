using AgregaNews.AnalyzeNews.Domain.Contracts.Services;
using AgregaNews.Common.Contracts.EventBus;
using AgregaNews.Common.Contracts.QueueEvents;
using AgregaNews.Common.Enums;
using Microsoft.Extensions.Configuration;
using OpenAI_API;

namespace AgregaNews.AnalyzeNews.Infrastructure.Services;

public class ChatGPTService : IChatGPTService
{
    private readonly string _apiKey;
    private readonly IEventBus _eventBus;

    public ChatGPTService(IConfiguration configuration, IEventBus eventBus)
    {
        var apiKey = configuration.GetSection("OpenAI:ApiKey").Value;
        if (apiKey is null)
        {
            throw new ArgumentNullException(nameof(apiKey));
        }

        _apiKey = apiKey;
        _eventBus = eventBus;
    }

    public async Task<string> UseChatGPT(string query)
    {
        try
        {
            var openAi = new OpenAIAPI(_apiKey);

            var outputResult = await openAi.Completions.GetCompletion(query);
            
            return outputResult;
        }
        catch (Exception ex)
        {
            await _eventBus.PublishAsync(new LogEvent()
            {
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
                Message = ex.Message,
                OccurredIn = DateTimeOffset.Now,
                Service = InfrastructureAssemblyReference.AssemblyName,
                Severity = LogSeverityEnum.Error,
                ExceptionType = nameof(ex),
                StackTrace = ex.StackTrace,
            });

            return "Geral";
        }
    }
}
