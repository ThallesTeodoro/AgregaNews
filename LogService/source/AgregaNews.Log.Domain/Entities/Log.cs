using AgregaNews.Common.Contracts.Data;

namespace AgregaNews.Log.Domain.Entities;

public class Log : IEntity
{
    public Guid Id { get; set; }
    public required string Message { get; set; }
    public required string Service { get; set; }
    public required string Severity { get; set; }
    public required string Environment { get; set; }
    public string? StackTrace { get; set; }
    public string? ExceptionType { get; set; }
    public required DateTimeOffset OccurredIn { get; set; }
}
