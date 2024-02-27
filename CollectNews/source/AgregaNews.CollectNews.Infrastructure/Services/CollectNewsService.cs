using AgregaNews.CollectNews.Domain.Contracts.Services;
using AgregaNews.CollectNews.Domain.DTOs;
using AgregaNews.CollectNews.Domain.Exceptions.CollectNews;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;

namespace AgregaNews.CollectNews.Infrastructure.Services;

public class CollectNewsService : ICollectNewsService
{
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly ILogger<CollectNewsService> _logger;

    public CollectNewsService(IConfiguration configuration, ILogger<CollectNewsService> logger)
    {
        var apiKey = configuration.GetSection("NewsApiSettings:ApiKey").Value;
        var baseUrl = configuration.GetSection("NewsApiSettings:BaseUrl").Value;

        if (apiKey is null)
        {
            throw new ArgumentNullException(nameof(apiKey));
        }

        if (baseUrl is null)
        {
            throw new ArgumentNullException(nameof(baseUrl));
        }

        _apiKey = apiKey;
        _baseUrl = baseUrl;
        _logger = logger;
    }

    public async Task<NewsDto?> CollectTopHeadlinesAsync(string country, string? category, int? pageSize, int? page)
    {
        try
		{
            HttpClient client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
            client.DefaultRequestHeaders.Add("user-agent", "News-API-csharp/0.1");
            client.DefaultRequestHeaders.Add("x-api-key", _apiKey);

            pageSize ??= 10;
            page ??= 1;

            var queryParams = new List<string>()
            {
                $"country={country}",
                $"pageSize={pageSize}",
                $"page={page}",
            };

            if (!string.IsNullOrEmpty(category))
            {
                queryParams.Add($"category={category}");
            }

            var querystring = string.Join("&", queryParams.ToArray());

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/top-headlines?{querystring}");
            var httpResponse = await client.SendAsync(httpRequest);

            var json = await httpResponse.Content.ReadAsStringAsync();

            if (!string.IsNullOrWhiteSpace(json))
            {
                var news = JsonConvert.DeserializeObject<NewsDto>(json);

                if (news?.status == "ok")
                {
                    return news;
                }

                throw new NewsStatusCodeErrorException("News status code is not 'ok'.");
            }

            return null;
        }
        catch (NewsStatusCodeErrorException)
        {
            _logger.LogError(
                "Service failure {@Error}, {@DateTimeUtc}",
                typeof(NewsStatusCodeErrorException).Name,
                DateTime.UtcNow);
            
            throw;
        }
		catch (Exception)
		{
			throw new NewsEndpointErrorException();
		}
    }
}
