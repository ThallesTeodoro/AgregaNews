using AgregaNews.CollectNews.Domain.Contracts.Services;
using AgregaNews.CollectNews.Domain.DTOs;
using AgregaNews.CollectNews.Domain.Exceptions.CollectNews;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;

namespace AgregaNews.CollectNews.Infrastructure.Services;

public class CollectNewsService : ICollectNewsService
{
    private readonly string ApiKey;
    private readonly string BaseUrl;

    public CollectNewsService(IConfiguration configuration)
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

        ApiKey = apiKey;
        BaseUrl = baseUrl;
    }

    public async Task<NewsDto?> CollectTopHeadlines(string country, string? category, int? pageSize, int? page)
    {
        try
		{
            HttpClient client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
            client.DefaultRequestHeaders.Add("user-agent", "News-API-csharp/0.1");
            client.DefaultRequestHeaders.Add("x-api-key", ApiKey);

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

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/top-headlines?{querystring}");
            var httpResponse = await client.SendAsync(httpRequest);

            var json = await httpResponse.Content.ReadAsStringAsync();

            if (!string.IsNullOrWhiteSpace(json))
            {
                var news = JsonConvert.DeserializeObject<NewsDto>(json);

                if (news?.status == "ok")
                {
                    return news;
                }

                throw new NewsStatusCodeErrorException();
            }

            return null;
        }
		catch (Exception)
		{
			throw new NewsEndpointErrorException();
		}
    }
}
