namespace AgregaNews.Log.Domain.DTOs;

public class Pagination<T> where T : class
{
    public int CurrentPage { get; set; }
    public int Total { get; set; }
    public required IReadOnlyList<T> Items { get; set; }
}
