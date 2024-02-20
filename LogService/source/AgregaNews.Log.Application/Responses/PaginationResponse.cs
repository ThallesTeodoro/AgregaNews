namespace AgregaNews.Log.Application.Responses;

public class PaginationResponse<T> where T : class
{
    public int CurrentPage { get; set; }
    public int Total { get; set; }
    public required IReadOnlyList<T> Items { get; set; }
}
