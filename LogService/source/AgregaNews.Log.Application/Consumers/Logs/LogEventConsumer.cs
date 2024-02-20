using AgregaNews.Common.Contracts.QueueEvents;
using AgregaNews.Log.Domain.Contracts.Repositories;
using AutoMapper;
using MassTransit;

namespace AgregaNews.Log.Application.Consumers.Logs;

public sealed class LogEventConsumer : IConsumer<LogEvent>
{
    private readonly ILogRepository _logRepository;
    private readonly IMapper _mapper;

    public LogEventConsumer(ILogRepository logRepository, IMapper mapper)
    {
        _logRepository = logRepository;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<LogEvent> context)
    {
        var logMessage = context.Message;

        try
        {
            var log = _mapper.Map<Domain.Entities.Log>(logMessage);
            log.Id = Guid.NewGuid();
            await _logRepository.AddAsync(log);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
