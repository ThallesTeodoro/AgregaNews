using AgregaNews.CollectNews.Application.Responses;
using AgregaNews.CollectNews.Domain.Contracts.QueueEvents;
using AgregaNews.CollectNews.Domain.DTOs;
using AgregaNews.CollectNews.Domain.Entities;
using AutoMapper;

namespace AgregaNews.CollectNews.Application.Mappers;

public class CollectNewsProfile : Profile
{
    public CollectNewsProfile()
    {
        CreateMap<ArticleDto, CollectNewsResponse>()
            .ConstructUsing(c => new CollectNewsResponse(
                Guid.NewGuid(),
                c.Author,
                c.Title,
                c.Description,
                c.Url,
                c.UrlToImage,
                c.PublishedAt,
                c.Content));

        CreateMap<ArticleDto, News>();
        CreateMap<ArticleDto, NewsAnalyzeEvent>();
    }
}
