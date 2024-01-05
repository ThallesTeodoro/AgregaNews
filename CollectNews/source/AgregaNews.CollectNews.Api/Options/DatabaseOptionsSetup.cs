using Microsoft.Extensions.Options;

namespace AgregaNews.CollectNews.Api.Options;

public class DatabaseOptionsSetup : IConfigureOptions<DatabaseOptions>
{
    private readonly IConfiguration _configuration;
    private const string ConfigurationSectionName = "CollectNewsDatabase";

    public DatabaseOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(DatabaseOptions options)
    {
        _configuration.GetSection(ConfigurationSectionName).Bind(options);
    }
}
