namespace AgregaNews.CollectNews.Domain.Contracts.QueueEvents;

public record NewsAnalyzeEvent
{
    public Guid Id { get; set; }
    public string? Author { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? UrlToImage { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public string? Content { get; set; }
}
