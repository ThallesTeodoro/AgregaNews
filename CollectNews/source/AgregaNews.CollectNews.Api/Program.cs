using AgregaNews.CollectNews.Api.HealthChecks;
using AgregaNews.CollectNews.Api.Middlewares;
using AgregaNews.CollectNews.Api.Options;
using AgregaNews.CollectNews.Application;
using AgregaNews.CollectNews.Infrastructure;
using AgregaNews.Common.Infrastructure.MessageBroker;
using Carter;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Serilog;

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
        Title = "AgregaNews - Collect News API",
        Version = "v1",
        Description = "API para coleta de notícias de diversas fontes",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "AgregaNews Team"
        }
    });
    
    // Habilitar Swagger em produção com autenticação opcional
    options.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "API Key Authentication",
        Name = "X-API-Key",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<MongoDbHealthCheck>("mongodb", tags: new[] { "ready", "mongodb" });

builder.Services.AddCarter();

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
                ?? new[] { "*" };
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
    });
});

builder.Host.UseSerilog((context, configuration) => 
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AgregaNews Collect News API v1");
    c.RoutePrefix = string.Empty; // Swagger UI na raiz
    if (!app.Environment.IsDevelopment())
    {
        c.EnablePersistAuthorization();
    }
});

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseMiddleware<ResponseContentTypeMiddleware>();

app.UseCors();

app.UseHttpsRedirection();

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
