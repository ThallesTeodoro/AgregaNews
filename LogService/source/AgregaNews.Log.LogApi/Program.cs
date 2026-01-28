using AgregaNews.Common.Infrastructure.MessageBroker;
using AgregaNews.Log.Infrastructure;
using AgregaNews.Log.Application;
using AgregaNews.Log.LogApi.HealthChecks;
using AgregaNews.Log.LogApi.Middlewares;
using AgregaNews.Log.LogApi.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Carter;

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
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "AgregaNews - Log Service API",
        Version = "v1",
        Description = "API para serviços de log e auditoria",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "AgregaNews Team"
        }
    });
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<MongoDbHealthCheck>("mongodb", tags: new[] { "ready", "mongodb" });

builder.Services.AddCarter();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AgregaNews Log Service API v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseMiddleware<ResponseContentTypeMiddleware>();

app.MapCarter();

// Health Check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false
});

app.Run();
