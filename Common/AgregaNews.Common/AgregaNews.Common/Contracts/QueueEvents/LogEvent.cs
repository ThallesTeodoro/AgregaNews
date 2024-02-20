namespace AgregaNews.Common.Contracts.QueueEvents;

public record LogEvent
{
    public required string Message { get; set; }
    public required string Service { get; set; }
    public required string Severity { get; set; }
    public required string Environment { get; set; }
    public string? StackTrace { get; set; }
    public string? ExceptionType { get; set; }
    public required DateTimeOffset OccurredIn { get; set; }
}
