using AgregaNews.AnalyzeNews.Domain.Contracts.Repositories;
using AgregaNews.AnalyzeNews.Domain.Contracts.Services;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using AgregaNews.AnalyzeNews.Application.Consumers.NewsAnalyze;
using MassTransit;
using MassTransit.TestFramework.ForkJoint.Contracts;
using MassTransit.Testing;
using AgregaNews.Common.Contracts.QueueEvents;
using Bogus;
using AgregaNews.AnalyzeNews.Application.Responses;
using AgregaNews.AnalyzeNews.Domain.Entities;
using System.Drawing;
using System.Diagnostics.Contracts;

namespace AgregaNews.AnalyzeNews.Application.UnitTests.Consumers.NewsAnalyze;

public class NewsAnalyzeEventConsumerTests
{
    private readonly List<string> categories = new List<string>()
    {
        "Negócios",
        "Entretenimento",
        "Geral",
        "Saúde",
        "Ciência",
        "Esportes",
        "Tecnologia",
    };
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IAnalyzedNewsRepository> _analyzedNewsRepositoryMock;
    private readonly Mock<IChatGPTService> _chatGPTServiceMock;
    private readonly Mock<ConsumeContext<NewsAnalyzeEvent>> _consumerContext;

    public NewsAnalyzeEventConsumerTests()
    {
        _mapperMock = new();
        _analyzedNewsRepositoryMock = new();
        _chatGPTServiceMock = new();
        _consumerContext = new();
    }

    [Fact]
    public async Task Consume_Should_SaveAnalyzedNews_WhenConsumeQueue()
    {
        // Arrange
        Random rand = new Random();
        _chatGPTServiceMock
            .Setup(x => x.UseChatGPT(It.IsAny<string>()))
            .ReturnsAsync(categories[rand.Next(0, 6)]);
        
        var news = MockNewsEvent();
        _mapperMock.Setup(
            x => x.Map<AnalyzedNews>(news))
            .Returns(MapMockedAnalyzedNews(news));

        _analyzedNewsRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<AnalyzedNews>()))
            .Returns(Task.CompletedTask);

        var consumer = new NewsAnalyzeEventConsumer(
            _chatGPTServiceMock.Object, 
            _analyzedNewsRepositoryMock.Object, 
            _mapperMock.Object);

        _consumerContext.SetupGet(x => x.Message).Returns(news);
        
        // Act
        await consumer.Consume(_consumerContext.Object);

        // Assert
        _analyzedNewsRepositoryMock.Verify(
            x => x.AddAsync(It.Is<AnalyzedNews>(n => n.Id == news.Id)),
            Times.Once);
    }

    private AnalyzedNews MapMockedAnalyzedNews(NewsAnalyzeEvent news)
    {
        return new AnalyzedNews()
        {
            Id = news.Id,
            Category = string.Empty,
            Author = news.Author,
            Title = news.Title,
            Description = news.Description,
            Url = news.Url,
            UrlToImage = news.UrlToImage,
            PublishedAt = news.PublishedAt,
            Content = news.Content,
        };
    }

    private NewsAnalyzeEvent MockNewsEvent()
    {
        return new Faker<NewsAnalyzeEvent>()
            .RuleFor(n => n.Id, Guid.NewGuid())
            .RuleFor(a => a.Author, f => f.Name.FullName())
            .RuleFor(a => a.Title, f => f.Lorem.Sentence(5))
            .RuleFor(a => a.Description, f => f.Lorem.Paragraph())
            .RuleFor(a => a.Url, "https://loremipsum.com")
            .RuleFor(a => a.UrlToImage, "https://loremipsum.com/image.png")
            .RuleFor(a => a.PublishedAt, DateTimeOffset.Now)
            .RuleFor(a => a.Content, string.Empty)
            .Generate();
    }
}
