using Microsoft.AspNetCore.Mvc;
using YoutubeCompanion.Infrastructure.YouTube;

namespace YoutubeCompanion.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly YouTubeAuthService _auth;

    public AuthController(YouTubeAuthService auth)
    {
        _auth = auth;
    }

    // React calls this
    [HttpGet("login")]
    public IActionResult Login()
    {
        var url = _auth.GetLoginUrl();
        return Redirect(url);
    }

    // Google redirects here
    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string code)
    {
        var credential = await _auth.ExchangeCodeForTokenAsync(code);

        if (string.IsNullOrEmpty(code))
        {
            return BadRequest("Missing authorization code");
        }

        _auth.CacheCredential(credential);

        Console.WriteLine($"Granted scopes: {credential.Token.Scope}");

        return Ok("YouTube OAuth successful");
    }
}