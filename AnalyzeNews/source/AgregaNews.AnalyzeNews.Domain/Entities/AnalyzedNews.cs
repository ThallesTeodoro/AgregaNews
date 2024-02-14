﻿using AgregaNews.Common.Contracts.Data;

namespace AgregaNews.AnalyzeNews.Domain.Entities;

public class AnalyzedNews : IEntity
{
    public Guid Id { get; set; }
    public string? Author { get; set; }
    public string? Title { get; set; }
    public required string Category { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? UrlToImage { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public string? Content { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public bool Collected { get; set; } = false;
}
