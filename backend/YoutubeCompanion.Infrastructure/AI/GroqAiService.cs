using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace YoutubeCompanion.Infrastructure.AI;

public class GroqAiService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public GroqAiService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    public async Task<List<string>> SuggestTitlesAsync(string currentTitle)
    {
        var payload = new
        {
            model = _config["Groq:Model"],
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = "You are an expert YouTube growth strategist."
                },
                new
                {
                    role = "user",
                    content = $"""
                    Improve the following YouTube video title.
                    Return exactly 3 improved, catchy title suggestions.
                    One title per line. No numbering.

                    Title: "{currentTitle}"
                    """
                }
            },
            temperature = 0.8
        };

        var apiKey = _config["Groq:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("Groq API key is not configured. Set the GROQ_API_KEY environment variable or put it in appsettings.Development.json under 'Groq:ApiKey'.");
        }

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.groq.com/openai/v1/chat/completions"
        );

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        request.Content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        var content = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return content!
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Take(3)
            .ToList();
    }
}