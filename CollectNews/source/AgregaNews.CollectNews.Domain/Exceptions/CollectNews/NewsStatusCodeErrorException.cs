namespace AgregaNews.CollectNews.Domain.Exceptions.CollectNews;

public class NewsStatusCodeErrorException : Exception
{
    public NewsStatusCodeErrorException(string message)
        : base(message)
    {
    }
}
