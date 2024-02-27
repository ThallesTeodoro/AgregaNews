using AgregaNews.AnalyzeNews.Application.Queries.AnalyzeNews.Analyzed;
using AgregaNews.AnalyzeNews.Application.Responses;
using AgregaNews.AnalyzeNews.Domain.Contracts.Repositories;
using AgregaNews.AnalyzeNews.Domain.Entities;
using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;

namespace AgregaNews.AnalyzeNews.Application.UnitTests.Queries.AnalyzeNews;

public class AnalyzedNewsQueryHandlerTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IAnalyzedNewsRepository> _analyzedNewsRepositoryMock;

    public AnalyzedNewsQueryHandlerTests()
    {
        _mapperMock = new();
        _analyzedNewsRepositoryMock = new();
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyCollection_WhenThereIsNoAnalyzedNews()
    {
        // Arrange
        var query = new AnalyzedNewsQuery(10);

        var news = new List<AnalyzedNews>();
        _analyzedNewsRepositoryMock
            .Setup(x => x.GetRecentAsync(It.IsAny<int>()))
            .ReturnsAsync(news);

        _mapperMock.Setup(
            x => x.Map<List<AnalyzedNewsResponse>>(news))
            .Returns(MapMockedAnalyzedNewsResponse(news));

        var handler = new AnalyzedNewsQueryHandler(_analyzedNewsRepositoryMock.Object, _mapperMock.Object);

        // Act
        List<AnalyzedNewsResponse> result = await handler.Handle(query, default);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnCollection_WhenThereIsAnalyzedNews()
    {
        // Arrange
        Random random = new Random();
        var size = random.Next(1, 10);
        var query = new AnalyzedNewsQuery(size);

        var analyzedNews = CreateMockedAnalyzedNews(size);
        _analyzedNewsRepositoryMock
            .Setup(x => x.GetRecentAsync(It.IsAny<int>()))
            .ReturnsAsync(analyzedNews);

        _mapperMock.Setup(
            x => x.Map<List<AnalyzedNewsResponse>>(analyzedNews))
            .Returns(MapMockedAnalyzedNewsResponse(analyzedNews));

        var handler = new AnalyzedNewsQueryHandler(_analyzedNewsRepositoryMock.Object, _mapperMock.Object);

        // Act
        List<AnalyzedNewsResponse> result = await handler.Handle(query, default);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(size);
    }

    private List<AnalyzedNews> CreateMockedAnalyzedNews(int size)
    {
        return new Faker<AnalyzedNews>()
            .RuleFor(a => a.Id, Guid.NewGuid())
            .RuleFor(a => a.Author, f => f.Name.FullName())
            .RuleFor(a => a.Title, f => f.Lorem.Sentence(5))
            .RuleFor(a => a.Category, f => "Geral")
            .RuleFor(a => a.Description, f => f.Lorem.Paragraph())
            .RuleFor(a => a.Url, "https://loremipsum.com")
            .RuleFor(a => a.UrlToImage, "https://loremipsum.com/image.png")
            .RuleFor(a => a.PublishedAt, DateTimeOffset.Now)
            .RuleFor(a => a.Content, string.Empty)
            .RuleFor(a => a.CreatedAt, DateTimeOffset.Now.AddDays(-5))
            .RuleFor(a => a.Collected, true)
            .Generate(size);
    }

    private List<AnalyzedNewsResponse> MapMockedAnalyzedNewsResponse(List<AnalyzedNews> analyzedNews)
    {
        return analyzedNews
            .Select(a => new AnalyzedNewsResponse(
                a.Id,
                a.Author,
                a.Title,
                a.Category,
                a.Description,
                a.Url,
                a.UrlToImage,
                a.PublishedAt,
                a.Content,
                a.CreatedAt,
                a.Collected))
            .ToList();
    }
}
