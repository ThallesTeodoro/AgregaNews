var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "AgregaNews - Gateway API",
        Version = "v1",
        Description = "API Gateway para os serviços AgregaNews",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "AgregaNews Team"
        }
    });
});

// Health Checks
builder.Services.AddHealthChecks();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AgregaNews Gateway API v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.MapReverseProxy();

// Health Check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false
});

app.Run();
