using AgregaNews.AnalyzeNews.Domain.Contracts.Repositories;
using AgregaNews.AnalyzeNews.Domain.Contracts.Services;
using AgregaNews.AnalyzeNews.Domain.Entities;
using AgregaNews.Common.Contracts.QueueEvents;
using AutoMapper;
using MassTransit;
using System.Text.RegularExpressions;

namespace AgregaNews.AnalyzeNews.Application.Consumers.NewsAnalyze;

public sealed class NewsAnalyzeEventConsumer : IConsumer<NewsAnalyzeEvent>
{
    private readonly IChatGPTService _chatGPTService;
    private readonly IAnalyzedNewsRepository _analyzedNewsRepository;
    private readonly IMapper _mapper;
    private readonly List<string> categories = new List<string>()
    {
        "Negócios",
        "Entretenimento",
        "Geral",
        "Saúde",
        "Ciência",
        "Esportes",
        "Tecnologia",
    };

    public NewsAnalyzeEventConsumer(IChatGPTService chatGPTService, IAnalyzedNewsRepository analyzedNewsRepository, IMapper mapper)
    {
        _chatGPTService = chatGPTService;
        _analyzedNewsRepository = analyzedNewsRepository;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<NewsAnalyzeEvent> context)
    {
        var collectedMessage = context.Message;

        if (collectedMessage is not null)
        {
            var prompt = $"Em qual das seguintes categorias o título de notícia \"{collectedMessage.Title}\" melhor se enquadra: " +
                $"{string.Join(", ", categories)}. Responda em uma só palavra";

            var category = await _chatGPTService.UseChatGPT(prompt);

            if (!string.IsNullOrEmpty(category))
            {
                var analyzedNews = _mapper.Map<AnalyzedNews>(collectedMessage);
                analyzedNews.Category = GetCategory(category);
                analyzedNews.CreatedAt = DateTimeOffset.Now;
                await _analyzedNewsRepository.AddAsync(analyzedNews);
            }
        }
    }

    private string GetCategory(string input)
    {
        foreach (var category in categories)
        {
            Match match = Regex.Match(input, "\\b" + Regex.Escape(category) + "\\b", RegexOptions.IgnoreCase);

            if (match.Success)
            {
                return match.Value;
            }
        }

        return "Geral";
    }
}
