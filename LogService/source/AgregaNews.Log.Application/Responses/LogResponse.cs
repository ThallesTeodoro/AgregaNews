namespace AgregaNews.Log.Application.Responses;

public class LogResponse
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
