using Microsoft.AspNetCore.Mvc;
using YoutubeCompanion.Infrastructure.AI;
using YoutubeCompanion.Infrastructure.YouTube;

namespace YoutubeCompanion.API.Controllers;

[ApiController]
[Route("api/ai")]
public class AiController : ControllerBase
{
    private readonly GroqAiService _ai;
    private readonly YouTubeAuthService _yt;
    private readonly IConfiguration _config;

    public AiController(
        GroqAiService ai,
        YouTubeAuthService yt,
        IConfiguration config)
    {
        _ai = ai;
        _yt = yt;
        _config = config;
    }

    [HttpGet("suggest-titles")]
    public async Task<IActionResult> SuggestTitles()
    {
        var credential = _yt.GetCachedCredential();
        var youtube = _yt.CreateYouTubeService(credential);

        var request = youtube.Videos.List("snippet");
        request.Id = _config["YouTube:VideoId"];

        var response = await request.ExecuteAsync();
        var title = response.Items.First().Snippet.Title;

        var suggestions = await _ai.SuggestTitlesAsync(title);

        return Ok(suggestions);
    }
}