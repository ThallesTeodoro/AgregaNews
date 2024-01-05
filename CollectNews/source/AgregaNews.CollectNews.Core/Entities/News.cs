using AgregaNews.CollectNews.Domain.Contracts.Data;

namespace AgregaNews.CollectNews.Domain.Entities;

public class News : IEntity
{
    public Guid Id { get; set; }
    public string? Author { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? UrlToImage { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public string? Content { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
