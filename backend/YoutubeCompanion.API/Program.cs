using YoutubeCompanion.Infrastructure.AI;
using YoutubeCompanion.Infrastructure.Mongo;
using YoutubeCompanion.Infrastructure.Settings;
using YoutubeCompanion.Infrastructure.YouTube;

var builder = WebApplication.CreateBuilder(args);

// 🔹 ADD THIS (CRITICAL)
builder.Services.AddControllers();

//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MongoDB settings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDb"));

builder.Services.AddSingleton<MongoContext>();

// YouTube OAuth service
builder.Services.AddSingleton<YouTubeAuthService>();
builder.Services.AddHttpClient<GroqAiService>();

builder.Services.AddSingleton<EventLogger>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000"             // local React
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS MUST come before UseHttpsRedirection and routing
app.UseCors("FrontendPolicy");

// HTTPS
app.UseHttpsRedirection();

// 🔹 ADD THIS (CRITICAL)
app.MapControllers();

// (Optional) Minimal API – can stay
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild",
    "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

// Config sanity check (non-secret): shows redirect URI and whether keys are present
app.MapGet("/api/config-check", (IConfiguration config) =>
{
    return new
    {
        youtubeRedirect = config["YouTube:RedirectUri"],
        youtubeClientIdSet = !string.IsNullOrWhiteSpace(config["YouTube:ClientId"]),
        youtubeClientSecretSet = !string.IsNullOrWhiteSpace(config["YouTube:ClientSecret"]),
        mongoConnectionSet = !string.IsNullOrWhiteSpace(config["MongoDb:ConnectionString"]),
        groqApiKeySet = !string.IsNullOrWhiteSpace(config["Groq:ApiKey"])
    };
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}