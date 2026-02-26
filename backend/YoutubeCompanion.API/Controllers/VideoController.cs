using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Mvc;
using YoutubeCompanion.Application.DTOs;
using YoutubeCompanion.Infrastructure.YouTube;

namespace YoutubeCompanion.API.Controllers;

[ApiController]
[Route("api/video")]
public class VideoController : ControllerBase
{
    private readonly YouTubeAuthService _auth;
    private readonly IConfiguration _config;
    private readonly EventLogger _eventLogger;

    public VideoController(
        YouTubeAuthService auth,
        IConfiguration config,
        EventLogger eventLogger)
    {
        _auth = auth;
        _config = config;
        _eventLogger = eventLogger;
    }

    [HttpGet("details")]
    public async Task<IActionResult> GetVideoDetails()
    {
        try
        {
            var credential = _auth.GetCachedCredential();
            var youtube = _auth.CreateYouTubeService(credential);

            var videoId = _config["YouTube:VideoId"];
            var details = await _auth.GetVideoDetailsAsync(youtube, videoId);

            return Ok(details);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateVideo(
        [FromBody] UpdateVideoDto request)
    {
        try
        {
            var credential = _auth.GetCachedCredential();
            var youtube = _auth.CreateYouTubeService(credential);

            var videoId = _config["YouTube:VideoId"];

            await _auth.UpdateVideoAsync(
                youtube,
                videoId,
                request.Title,
                request.Description
            );

            await _eventLogger.LogAsync("VideoUpdated", new
            {
                videoId,
                title = request.Title,
                description = request.Description
            });

            return Ok(new
            {
                message = "Video updated successfully"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("comment")]
    public async Task<IActionResult> AddComment(
    [FromBody] AddCommentDto request)
    {
        try
        {
            // Ensure the cached credential exists and has the required scope for write operations.
            var credential = _auth.GetCachedCredential();
            try
            {
                _auth.EnsureCachedCredentialHasScopes(YouTubeService.Scope.YoutubeForceSsl);
            }
            catch (Exception scopeEx)
            {
                return StatusCode(403, new { message = scopeEx.Message });
            }
            var youtube = _auth.CreateYouTubeService(credential);
            var videoId = _config["YouTube:VideoId"];

            var commentId = await _auth.AddCommentAsync(
                youtube, videoId, request.Text);

            await _eventLogger.LogAsync("CommentAdded", new
            {
                videoId,
                comment = request.Text
            });

            return Ok(new
            {
                message = "Comment added",
                commentId
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("comment/reply")]
    public async Task<IActionResult> ReplyToComment(
    [FromBody] ReplyCommentDto request)
    {
        try
        {
            var credential = _auth.GetCachedCredential();
            try
            {
                _auth.EnsureCachedCredentialHasScopes(YouTubeService.Scope.YoutubeForceSsl);
            }
            catch (Exception scopeEx)
            {
                return StatusCode(403, new { message = scopeEx.Message });
            }
            var youtube = _auth.CreateYouTubeService(credential);

            var replyId = await _auth.ReplyToCommentAsync(
                youtube,
                request.ParentCommentId,
                request.Text);

            return Ok(new
            {
                message = "Reply added",
                replyId
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("comment/{commentId}")]
    public async Task<IActionResult> DeleteComment(string commentId)
    {
        try
        {
            var credential = _auth.GetCachedCredential();
            try
            {
                _auth.EnsureCachedCredentialHasScopes(YouTubeService.Scope.YoutubeForceSsl);
            }
            catch (Exception scopeEx)
            {
                return StatusCode(403, new { message = scopeEx.Message });
            }
            var youtube = _auth.CreateYouTubeService(credential);

            await _auth.DeleteCommentAsync(youtube, commentId);

            return Ok(new
            {
                message = "Comment deleted"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}