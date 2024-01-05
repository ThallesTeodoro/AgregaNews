using AgregaNews.CollectNews.Application.Queries.CollectNews.Collect;
using AgregaNews.CollectNews.Application.Responses;
using AgregaNews.CollectNews.Domain.Contracts.EventBus;
using AgregaNews.CollectNews.Domain.Contracts.QueueEvents;
using AgregaNews.CollectNews.Domain.Contracts.Repositories;
using AgregaNews.CollectNews.Domain.Contracts.Services;
using AgregaNews.CollectNews.Domain.DTOs;
using AgregaNews.CollectNews.Domain.Entities;
using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;

namespace AgregaNews.CollectNews.Application.UnitTests.Queries.CollectNews;

public class CollectNewsQueryHandlerTests
{
    private readonly Mock<ICollectNewsService> _collectNewsServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<INewsRepository> _newsRepositoryMock;
    private readonly Mock<IEventBus> _eventBusMock;

    public CollectNewsQueryHandlerTests()
    {
        _collectNewsServiceMock = new();
        _mapperMock = new();
        _newsRepositoryMock = new();
        _eventBusMock = new();
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyCollection_WhenCantCollectNews()
    {
        // Arrange
        var query = new CollectNewsQuery("br", null, 10, 1);

        _collectNewsServiceMock.Setup(
            x => x.CollectTopHeadlinesAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ReturnsAsync((NewsDto?)null);

        var handler = new CollectNewsQueryHandler(
            _collectNewsServiceMock.Object,
            _mapperMock.Object,
            _newsRepositoryMock.Object,
            _eventBusMock.Object);

        // Act
        List<CollectNewsResponse> result = await handler.Handle(query, default);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_CallAddOnRepository_WhenThereAreNews()
    {
        // Arrange
        var query = new CollectNewsQuery("br", null, 10, 1);

        var newsDto = CreateMockedNews(10);

        _collectNewsServiceMock.Setup(
            x => x.CollectTopHeadlinesAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ReturnsAsync(newsDto);

        _mapperMock.Setup(
            x => x.Map<List<CollectNewsResponse>>(
                newsDto.articles))
            .Returns(MapperMockedToCollectNewsResponse(newsDto))
            .Verifiable();

        var news = MapperMockedToNews(newsDto);
        _mapperMock.Setup(
            x => x.Map<List<News>>(
                newsDto.articles))
            .Returns(news)
            .Verifiable();

        var handler = new CollectNewsQueryHandler(
            _collectNewsServiceMock.Object,
            _mapperMock.Object,
            _newsRepositoryMock.Object,
            _eventBusMock.Object);

        // Act
        List<CollectNewsResponse> result = await handler.Handle(query, default);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(10);
        _newsRepositoryMock.Verify(
            x => x.AddManyAsync(It.Is<List<News>>(n => n == news)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_CallPublishOnEventBus_WhenThereAreNews()
    {
        // Arrange
        var query = new CollectNewsQuery("br", null, 10, 1);

        var newsDto = CreateMockedNews(10);

        _collectNewsServiceMock.Setup(
            x => x.CollectTopHeadlinesAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ReturnsAsync(newsDto);

        _mapperMock.Setup(
            x => x.Map<List<CollectNewsResponse>>(
                newsDto.articles))
            .Returns(MapperMockedToCollectNewsResponse(newsDto))
            .Verifiable();

        var news = MapperMockedToNews(newsDto);
        _mapperMock.Setup(
            x => x.Map<List<News>>(
                newsDto.articles))
            .Returns(news)
            .Verifiable();

        var newsEvent = MapperMockedToNewsAnalyzeEvent(newsDto);
        _mapperMock.Setup(
            x => x.Map<List<NewsAnalyzeEvent>>(
                newsDto.articles))
            .Returns(newsEvent)
            .Verifiable();

        var handler = new CollectNewsQueryHandler(
            _collectNewsServiceMock.Object,
            _mapperMock.Object,
            _newsRepositoryMock.Object,
            _eventBusMock.Object);

        // Act
        List<CollectNewsResponse> result = await handler.Handle(query, default);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(10);
        _eventBusMock.Verify(
            x => x.PublishAsync(It.Is<List<NewsAnalyzeEvent>>(n => n == newsEvent), default),
            Times.Once);
    }

    private NewsDto CreateMockedNews(int totalResults = 10)
    {
        var articles = new Faker<ArticleDto>()
            .RuleFor(a => a.Source, (SourceDto?)null)
            .RuleFor(a => a.Author, f => f.Name.FullName())
            .RuleFor(a => a.Title, f => f.Lorem.Sentence(5))
            .RuleFor(a => a.Description, f => f.Lorem.Paragraph())
            .RuleFor(a => a.Url, "https://loremipsum.com")
            .RuleFor(a => a.UrlToImage, "https://loremipsum.com/image.png")
            .RuleFor(a => a.PublishedAt, DateTimeOffset.Now)
            .RuleFor(a => a.Content, string.Empty)
            .Generate(totalResults);

        return new NewsDto("Ok", totalResults, articles);
    }

    private List<News> MapperMockedToNews(NewsDto news)
    {
        return news.articles
            .Select(n => new News()
            {
                Id = Guid.NewGuid(),
                Author = n.Author,
                Content = n.Content,
                CreatedAt = DateTimeOffset.Now,
                Description = n.Description,
                PublishedAt = n.PublishedAt,
                Title = n.Title,
                Url = n.Url,
                UrlToImage = n.UrlToImage,
            })
            .ToList();
    }

    private List<CollectNewsResponse> MapperMockedToCollectNewsResponse(NewsDto news)
    {
        return news.articles
            .Select(n => new CollectNewsResponse(
                Guid.NewGuid(),
                n.Author,
                n.Title,
                n.Description,
                n.Url,
                n.UrlToImage,
                n.PublishedAt,
                n.Content
            ))
            .ToList();
    }

    private List<NewsAnalyzeEvent> MapperMockedToNewsAnalyzeEvent(NewsDto news)
    {
        return news.articles
            .Select(n => new NewsAnalyzeEvent()
            {
                Id = Guid.NewGuid(),
                Author = n.Author,
                Content = n.Content,
                Description = n.Description,
                PublishedAt = n.PublishedAt,
                Title = n.Title,
                Url = n.Url,
                UrlToImage = n.UrlToImage,
            })
            .ToList();
    }
}
