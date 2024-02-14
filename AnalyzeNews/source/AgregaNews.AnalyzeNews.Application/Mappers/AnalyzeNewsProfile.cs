using AgregaNews.AnalyzeNews.Application.Responses;
using AgregaNews.AnalyzeNews.Domain.Entities;
using AgregaNews.Common.Contracts.QueueEvents;
using AutoMapper;

namespace AgregaNews.AnalyzeNews.Application.Mappers;

public class AnalyzeNewsProfile : Profile
{
    public AnalyzeNewsProfile()
    {
        CreateMap<NewsAnalyzeEvent, AnalyzedNews>();
        CreateMap<AnalyzedNews, AnalyzedNewsResponse>();
    }
}