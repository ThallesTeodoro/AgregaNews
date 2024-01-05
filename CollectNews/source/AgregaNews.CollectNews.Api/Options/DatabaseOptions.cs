﻿namespace AgregaNews.CollectNews.Api.Options;

public class DatabaseOptions
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string NewsCollectionName { get; set; } = null!;
}
