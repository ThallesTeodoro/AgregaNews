using AgregaNews.Common.Infrastructure.MessageBroker;
using Microsoft.Extensions.Options;
using AgregaNews.AnalyzeNews.Application;
using AgregaNews.AnalyzeNews.Infrastructure;
using AgregaNews.AnalyzeNews.Api.Options;
using MongoDB.Driver;
using Carter;
using AgregaNews.AnalyzeNews.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureOptions<DatabaseOptionsSetup>();

builder.Services.Configure<MessageBrokerSettings>(builder.Configuration.GetSection("MessageBroker"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<MessageBrokerSettings>>().Value);

builder.Services.AddSingleton<IMongoDatabase>(serviceProvider =>
{
    var databaseOptions = serviceProvider.GetService<IOptions<DatabaseOptions>>()!.Value;

    var mongoClient = new MongoClient(databaseOptions.ConnectionString);

    return mongoClient.GetDatabase(databaseOptions.DatabaseName);
});

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCarter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.MapCarter();

app.Run();