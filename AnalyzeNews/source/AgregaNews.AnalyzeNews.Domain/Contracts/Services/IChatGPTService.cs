namespace AgregaNews.AnalyzeNews.Domain.Contracts.Services;

public interface IChatGPTService
{
    Task<string> UseChatGPT(string query);
}