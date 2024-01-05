using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace AgregaNews.CollectNews.Api.Contracts;

public sealed class JsonResponse<TData, TError>
{
    [JsonProperty("statusCode")]
    public int StatusCode { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("data")]
    public TData? Data { get; set; }

    [JsonProperty("errors")]
    public TError? Errors { get; set; }

    public JsonResponse(int statusCode, TData? data, TError? errors)
    {
        StatusCode = statusCode;
        Message = ReasonPhrases.GetReasonPhrase(statusCode);
        Data = data;
        Errors = errors;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
        });
    }
}
