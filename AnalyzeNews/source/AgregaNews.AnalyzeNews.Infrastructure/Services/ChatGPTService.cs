using AgregaNews.AnalyzeNews.Domain.Contracts.Repositories;
using AgregaNews.AnalyzeNews.Domain.Contracts.Services;
using AgregaNews.AnalyzeNews.Domain.Entities;
using Microsoft.Extensions.Configuration;
using OpenAI_API;

namespace AgregaNews.AnalyzeNews.Infrastructure.Services;

public class ChatGPTService : IChatGPTService
{
    private readonly string _apiKey;
    private readonly ILogRepository _logRepository;

    public ChatGPTService(ILogRepository logRepository, IConfiguration configuration)
    {
        _logRepository = logRepository;

        var apiKey = configuration.GetSection("OpenAI:ApiKey").Value;
        if (apiKey is null)
        {
            throw new ArgumentNullException(nameof(apiKey));
        }

        _apiKey = apiKey;
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
            await _logRepository.AddAsync(new Log()
            {
                Id = Guid.NewGuid(),
                Environment = "Dev",
                Message = ex.Message,
                OccurredIn = DateTimeOffset.Now,
                Service = "AgregaNews.AnalyzeNews",
                Severity = "Error",
                ExceptionType = nameof(ex),
                StackTrace = ex.StackTrace,
            });

            throw;
        }
    }
}
