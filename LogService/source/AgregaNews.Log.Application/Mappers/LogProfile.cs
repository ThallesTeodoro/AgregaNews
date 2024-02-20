using AgregaNews.Common.Contracts.QueueEvents;
using AgregaNews.Log.Application.Responses;
using AutoMapper;

namespace AgregaNews.Log.Application.Mappers;
public class LogProfile : Profile
{
    public LogProfile()
    {
        CreateMap<LogEvent, Domain.Entities.Log>();
        CreateMap<Domain.Entities.Log, LogResponse>();
    }
}
